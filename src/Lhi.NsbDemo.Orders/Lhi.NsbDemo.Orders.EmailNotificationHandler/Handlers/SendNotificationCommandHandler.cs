using System;
using System.Threading.Tasks;
using Lhi.NsbDemo.Orders.Messages;
using NServiceBus;
using NServiceBus.Logging;
using Prometheus;

namespace Lhi.NsbDemo.Orders.EmailNotificationHandler.Handlers
{
    public class SendNotificationCommandHandler : IHandleMessages<SendNotificationCommand>
    {
        static ILog log = LogManager.GetLogger<SendNotificationCommandHandler>();

        private static Counter _promCounter = Metrics.CreateCounter("nsbdemo_notification_counter", "Email Notification Counter", labelNames: new[] { "host", "status" });

        public Task Handle(SendNotificationCommand message, IMessageHandlerContext context)
        {

            Console.WriteLine($"Step 4: {message.Message}");
            //log.Info($"Step 4: {message.Message}");

            var format = "{Application}.{Service}.{Operation}-{TransactionId}: {LogMessage}";
            var logMessage = $"Step 4: {message.Message}";
            try
            {
                log.InfoFormat(format, "Lhi.NsbDemo.Orders", "SendNotificationCommandHandler", "Handle-SendNotificationCommand", message.TransactionId, logMessage);
            }
            catch { }

            _promCounter.Labels(Environment.MachineName, "sent").Inc();

            return Task.CompletedTask;
        }
    }
}
