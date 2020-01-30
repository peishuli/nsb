using Lhi.NsbDemo.Orders.EmailNotificationHandler.Handlers;
using Lhi.NsbDemo.Orders.Messages;
using NServiceBus.Testing;
using System;
using Xunit;

namespace Lhi.NsbDemo.Orders.EmailNotificationHandler.Tests
{
    public class EmailNotificationHandlerTests
    {
        [Fact]
        public void This_Is_a_dummy_test()
        {
            var transactionId = Guid.NewGuid().ToString();

            Test.Handler<SendNotificationCommandHandler>()
                .ExpectNotSend<SendNotificationCommand>(
                    check: message => {
                        return message.Message == "it doesn't really matters!";
                    }
                )
                .OnMessage<SendNotificationCommand>(
                    initializeMessage: message =>
                    {
                        message.TransactionId = transactionId;
                        message.Message = "Hello world!";
                    }
                );         
        }       
    }
}
