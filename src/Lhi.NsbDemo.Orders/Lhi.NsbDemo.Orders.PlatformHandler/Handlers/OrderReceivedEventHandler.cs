using Lhi.NsbDemo.Orders.Messages;
using NServiceBus;
using NServiceBus.Logging;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lhi.NsbDemo.Orders.PlanformHandlers.Handlers
{
    public class OrderReceivedCommandHandler : IHandleMessages<OrderReceivedEvent>
    {
        static private int count = 0;
        static ILog log = LogManager.GetLogger<OrderReceivedCommandHandler>();
        private static Counter _promCounter = Metrics.CreateCounter("nsbdemo_platform_event_counter", "Platform Event Counter", labelNames: new[] { "host", "status" });

        public Task Handle(OrderReceivedEvent message, IMessageHandlerContext context)
        {
            var format = "{Application}.{Service}.{Operation}-{TransactionId}: {LogMessage}";

            // simulate long running process
            Thread.Sleep(TimeSpan.FromSeconds(5));
            Console.WriteLine($"Step 2: Received OrderReceivedEvent {message.TransactionId}");
            //log.Info($"Step 2: Received OrderReceivedEvent {message.TransactionId}");

            
            var logMessage = $"Step 2: Received OrderReceivedEvent {message.TransactionId}";

            try
            {
                log.InfoFormat(format, "Lhi.NsbDemo.Orders", "OrderReceivedCommandHandler", "Handle-OrderProcessedEvent", message.TransactionId, logMessage);
            }
            catch { }

            _promCounter.Labels(Environment.MachineName, "received").Inc();

            //TODO: resolving backend platform and process the order
            var result = "succeeded";//happy scenario
           
            count++;
            // fail on every 3rd order received
            if (count == 3)
            {
                result = "failed";
                count = 0;
            }
            // Send process result back to the Saga
            var orderProcessedEvent = new OrderProcessedEvent
            {
                TransactionId = message.TransactionId,
                Results = result     
            };

            try
            {
                context.Publish(orderProcessedEvent);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Could not publish OrderProcessedEvent. Error: {ex.Message}";
                log.ErrorFormat(format, "Lhi.NsbDemo.Orders", "OrderReceivedCommandHandler", "Handle-OrderProcessedEvent", message.TransactionId, errorMessage);
            }
            return Task.CompletedTask;
        }
    }
}
