using Lhi.NsbDemo.Orders.EmailNotificationHandler.Utils;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Serilog;
using NServiceBus.Serilog.Tracing;
using Prometheus;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lhi.NsbDemo.Orders.EmailNotificationHandler
{
    class Program
    {
        static async Task Main()
        {
            Console.CancelKeyPress += OnExit;

            Console.Title = "Lhi.HsbDemo.Orders.EmailNotificationHandler";

            // Start Metrics for Prometheus
            //var metricServer = new MetricServer(port: 3031);
            var prometheus = AppSettings.Prometheus;
            var metricServer = new MetricServer(prometheus, 3031);
            metricServer.Start();

            var endpointConfiguration = new EndpointConfiguration("Lhi.NsbDemo.Orders.EmailNotificationHandler");

            // Configure Stackify
            StackifyLib.Config.ApiKey = AppSettings.StackifyApiKey; 
            StackifyLib.Config.AppName = AppSettings.StackifyApiName;
            StackifyLib.Config.Environment = AppSettings.StackifyEnvironment;
            // configure Serilog
            var tracingLog = new LoggerConfiguration()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(AppSettings.ElasticSearchUrl))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                IndexFormat = "nsbdemo-{0:yyyy.MM.dd}",
                InlineFields = true
            })
            .WriteTo.Stackify()
            .MinimumLevel.Information()
            .CreateLogger();
            var serilogFactory = LogManager.Use<SerilogFactory>();
            serilogFactory.WithLogger(tracingLog);
            endpointConfiguration.EnableFeature<TracingLog>();
            endpointConfiguration.SerilogTracingTarget(tracingLog);

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            var rabbitmqConnectionString = AppSettings.RabbitMQUrl;
            transport.ConnectionString(rabbitmqConnectionString);

            //endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

            endpointConfiguration.EnableInstallers();
            endpointConfiguration.Conventions()
               //.DefiningEventsAs(c => c.Namespace != null && c.Name.EndsWith("Event"))
               .DefiningCommandsAs(c => c.Namespace != null && c.Name.EndsWith("Command"));

            var endpointInstance = Endpoint.Start(endpointConfiguration)
                    .GetAwaiter().GetResult();

            // Wait until the message arrives.
            closingEvent.WaitOne();

            await endpointInstance.Stop()
                .ConfigureAwait(false);


        }

        static void OnExit(object sender, ConsoleCancelEventArgs args)
        {
            closingEvent.Set();
        }

        static AutoResetEvent closingEvent = new AutoResetEvent(false);
    }
}
