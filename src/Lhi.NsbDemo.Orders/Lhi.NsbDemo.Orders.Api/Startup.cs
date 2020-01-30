using Alexinea.Autofac.Extensions.DependencyInjection;
using Autofac;
using Lhi.NsbDemo.Orders.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using Prometheus;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Data.SqlClient;

namespace Lhi.NsbDemo.Orders.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var elasticsearchUrl = configuration.GetValue<string>("elasticsearch_url");

            // Configure Stackify
            var stackifyApiKay = configuration.GetValue<string>("Stackify.ApiKey");
            var stackifyApiName = configuration.GetValue<string>("Stackify.ApiName");
            var stackifyEnvironment = configuration.GetValue<string>("Stackify.Environment");
            StackifyLib.Config.ApiKey = stackifyApiKay;
            StackifyLib.Config.AppName = stackifyApiName;
            StackifyLib.Config.Environment = stackifyEnvironment;

            // Configure the Serilog logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                //.WriteTo.ColoredConsole()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                    IndexFormat = "nsbdemo-{0:yyyy.MM.dd}",
                    InlineFields = true
                })
                .WriteTo.Stackify()
                .CreateLogger();

            // Start Metric Server for Prometheus
            var prometheus = configuration.GetValue<string>("prometheus");
            //var metricServer = new MetricServer(port: 3030);
            var metricServer = new MetricServer(prometheus, 3030);
            metricServer.Start();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Lhi.NsbDemo.Orders.Api",
                    Version = "v1",
                    Description = "NAM Portal Orders API Demo",
                    TermsOfService = "Term of Service"

                });
            });

            var containerBuilder = new ContainerBuilder();

            containerBuilder.Populate(services);
            // NServiceBus
            var container = RegisterEventBus(containerBuilder);

            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add Serilog to the logging pipeline
            loggerFactory.AddSerilog();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API V1");
            });


            app.UseMvc();
        }

        private Autofac.IContainer RegisterEventBus(ContainerBuilder containerBuilder)
        {
            IEndpointInstance endpoint = null;
            containerBuilder.Register(c => endpoint)
                .As<IEndpointInstance>()
                .SingleInstance();

            var container = containerBuilder.Build();

            var endpointConfiguration = new EndpointConfiguration("Lhi.NsbDemo.Orders.Api");

            // Configure RabbitMQ transport
            var rabbitmqUrl = Configuration.GetValue<string>("rabbitmq_url");
            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            transport.ConnectionString(rabbitmqUrl);

            // Configure routing
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(CreateOrderCommand), "Lhi.NsbDemo.Orders.Sagas");

            // Configure SQL persistence
            var sqlConnection = Configuration.GetValue<string>("sql_connection_string");
            EnsureSqlDatabaseExists(sqlConnection);
            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(connectionBuilder:
                () => new SqlConnection(sqlConnection));

            // Use JSON.NET serializer
            //endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            // Enable the Outbox
            endpointConfiguration.EnableOutbox();

            // Make sure NServiceBus creates queues in RabbitMQ, turn it off in PROD
            endpointConfiguration.EnableInstallers();

            // Turn on auditing.
            endpointConfiguration.AuditProcessedMessagesTo("audit");

            // Define conventions
            endpointConfiguration.Conventions()
               //.DefiningEventsAs(c => c.Namespace != null && c.Name.EndsWith("Event"))
               .DefiningCommandsAs(c => c.Namespace != null && c.Name.EndsWith("Command"));

            // Configure the DI container.
            endpointConfiguration.UseContainer<AutofacBuilder>(customizations: customizations =>
            {
                customizations.ExistingLifetimeScope(container);
            });

            // Start the endpoint and register it with ASP.NET Core DI
            endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            return container;
        }

        private void EnsureSqlDatabaseExists(string sqlConnection)
        {
            var builder = new SqlConnectionStringBuilder(sqlConnection);
            var originalCatalog = builder.InitialCatalog;

            builder.InitialCatalog = "master";
            var masterConnectionString = builder.ConnectionString;

            using (var connection = new SqlConnection(masterConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{originalCatalog}')" +
                    $"  CREATE DATABASE [{originalCatalog}]";
                command.ExecuteNonQuery();
            }
        }
    }
}
