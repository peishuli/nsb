using System;
using System.Threading.Tasks;
using Lhi.NsbDemo.Orders.Messages;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Persistence.Sql;
using Prometheus;

namespace Lhi.NsbDemo.Orders.Sagas.Sagas
{
    public class OrdersSaga : SqlSaga<OrdersSagaData>,
        IAmStartedByMessages<CreateOrderCommand>,
        IHandleMessages<OrderProcessedEvent>
    {
        static ILog log = LogManager.GetLogger<OrdersSaga>();
        private static Counter _promCounter = Metrics.CreateCounter("nsbdemo_sagas_event_counter", "Sagas Event Counter", labelNames: new[] { "host", "status" });
        public Task Handle(CreateOrderCommand message, IMessageHandlerContext context)
        {
            var format = "{Application}.{Service}.{Operation}-{TransactionId}: {LogMessage}";

            // publish an OrderReceivedEvent via the context object
            var orderReceivedEvent = new OrderReceivedEvent
            {
                TransactionId = message.TransactionId
            };

            try
            {
                context.Publish(orderReceivedEvent);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Could not publish OrderReceivedEvent. Error: {ex.Message}";
                log.ErrorFormat(format, "Lhi.NsbDemo.Orders", "OrdersSaga", "Handle-CreatedOrderCommand", message.TransactionId, errorMessage);
            }
           

            Console.WriteLine($"Step 1: CreateOrderCommand {message.TransactionId} was received.");
            //log.Info($"Step 1: CreateOrderCommand {message.TransactionId} was received.");

            _promCounter.Labels(Environment.MachineName, "received").Inc();

            var logMessage = $"Step 1: CreateOrderCommand {message.TransactionId} was received.";
            try
            {
                log.InfoFormat(format, "Lhi.NsbDemo.Orders", "OrdersSaga", "Handle-CreatedOrderCommand", message.TransactionId, logMessage);
            }
            catch { }

            return Task.CompletedTask;
        }

        // ref: https://docs.particular.net/samples/saga/sql-sagafinder/
        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<CreateOrderCommand>(c => c.TransactionId);
            mapper.ConfigureMapping<OrderProcessedEvent>(e => e.TransactionId);
        }

        protected override string CorrelationPropertyName => nameof(OrdersSagaData.CorrelationId);

        public Task Handle(OrderProcessedEvent message, IMessageHandlerContext context)
        {
            // handle response
            Console.WriteLine($"Step 3: Order {message.TransactionId} has been process with results: {message.Results}");
            //log.Info($"Step 3: Order {message.TransactionId} has been process with results: {message.Results}");

            var format = "{Application}.{Service}.{Operation}-{TransactionId}: {LogMessage}";
            var logMessage = $"Step 3: Order {message.TransactionId} has been process with results: {message.Results}";
            try
            {
                log.InfoFormat(format, "Lhi.NsbDemo.Orders", "OrdersSaga", "Handle-OrderProcessedEvent", message.TransactionId, logMessage);
            }
            catch { }

            _promCounter.Labels(Environment.MachineName, "processed").Inc();

            // if order processing failed, send a notification command
            if (message.Results == "failed")
            {
                var sendNotificationCommand = new SendNotificationCommand
                {
                    TransactionId = message.TransactionId,
                    Message = $"Order {message.TransactionId} could not be procceeded. Please try again!"
                };

                try
                {
                    context.Send(sendNotificationCommand);
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Could not send SendNotificationCommand. Error: {ex.Message}";
                    log.ErrorFormat(format, "Lhi.NsbDemo.Orders", "OrdersSaga", "Handle-OrderProcessedEvent", message.TransactionId, errorMessage);
                }
            }

            return Task.CompletedTask;
        }

        
    }
}
