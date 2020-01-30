using Lhi.NsbDemo.Orders.Messages;
using Lhi.NsbDemo.Orders.PlanformHandlers.Handlers;
using NServiceBus.Testing;
using System;
using Xunit;

namespace Lhi.NsbDemo.Orders.OrderReceivedEventHandler.Tests
{
    public class OrderReceivedCommandHandlerTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Should_Publish_OrderProcessedEvent_With_Matching_TransactionIds_Async()
        {
            var transactionId = Guid.NewGuid().ToString();

            Test.Handler<OrderReceivedCommandHandler>()
                .ExpectPublish<OrderProcessedEvent>(
                    check: message =>
                    {
                        return message.TransactionId == transactionId 
                            && (message.Results == "succeeded"|| message.Results == "failed");
                    }
                )
                .ExpectNotPublish<OrderProcessedEvent>(
                    check: message =>
                    {
                        return message.TransactionId == "123";
                    }

                )
                .OnMessage<OrderReceivedEvent>(
                    initializeMessage: message  =>
                    {
                        message.TransactionId = transactionId;
                    })                
                ;
        }
    }
}
