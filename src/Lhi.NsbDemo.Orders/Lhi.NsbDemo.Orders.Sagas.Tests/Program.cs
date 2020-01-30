using Lhi.NsbDemo.Orders.Messages;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Lhi.NsbDemo.Orders.Sagas
{
    class Program
    {
        static async Task Main()
        {
            Console.CancelKeyPress += OnExit;

            Console.Title = "Lhi.HsbDemo.Orders.Sagas";
                                                                   
            var endpointConfiguration = new EndpointConfiguration("Lhi.NsbDemo.Orders.Sagas");

            var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
            var sqlConnection = @"Data Source=.\SqlExpress;Initial Catalog=NsbDemoSqlPersistence;Integrated Security=True";//TODO: from config
            persistence.SqlDialect<SqlDialect.MsSqlServer>();
            persistence.ConnectionBuilder(
                connectionBuilder: () =>
                {
                    return new SqlConnection(sqlConnection);
                });
            var subscriptions = persistence.SubscriptionSettings();
            subscriptions.CacheFor(TimeSpan.FromMinutes(1));
            SqlHelper.EnsureDatabaseExists(sqlConnection);

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
            transport.UseConventionalRoutingTopology();
            var rabbitmqConnectionString = "host=localhost;port=5672;username=guest;password=guest;";//TODO: read from config
            transport.ConnectionString(rabbitmqConnectionString);
            
            //endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
           
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.Conventions()
               .DefiningEventsAs(c => c.Namespace != null && c.Name.EndsWith("Event"))
               .DefiningCommandsAs(c => c.Namespace != null && c.Name.EndsWith("Command"));

            // Configure routing - this Saga will send an EmailNotificationCommand  when there is a failure in order processing
            var routing = transport.Routing();
            routing.RouteToEndpoint(typeof(SendNotificationCommand), "Lhi.NsbDemo.Orders.EmailNotificationHandler");

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
