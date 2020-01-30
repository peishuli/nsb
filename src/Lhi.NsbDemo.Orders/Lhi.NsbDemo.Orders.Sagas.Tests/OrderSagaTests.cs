using NServiceBus.Testing;
using Xunit;
using Lhi.NsbDemo.Orders.Messages;
using Lhi.NsbDemo.Orders.Sagas.Sagas;
using System;

namespace Lhi.NsbDemo.Orders.Sagas.Tests
{
    public class OrderSagaTests
    {
        [Fact]
        public void Should_pubish_OrderReceivedEvent_after_received_CreateOrderCommand()
        {
            var transactionId = Guid.NewGuid().ToString();
            Test.Saga<OrdersSaga>()
                .ExpectPublish<OrderReceivedEvent>(check: evt => evt.TransactionId == transactionId)
                .When((saga, context) => saga.Handle(
                    new CreateOrderCommand { TransactionId = transactionId }, context));
        }

        [Fact]
        public void Should_send_notification_when_process_failed()
        {
            var transactionId = Guid.NewGuid().ToString();
            var results = "failed";
            Test.Saga<OrdersSaga>()
                .ExpectSend<SendNotificationCommand>()
                .When((saga, context)=> saga.Handle(
                    new OrderProcessedEvent
                    {
                        TransactionId = transactionId,
                        Results = results
                    }, context));
        }

        [Fact]
        public void Should_not_send_notification_when_process_succeeded()
        {
            var transactionId = Guid.NewGuid().ToString();
            var results = "succeeded";
            Test.Saga<OrdersSaga>()
                .ExpectNotSend<SendNotificationCommand>()
                .When((saga, context) => saga.Handle(
                    new OrderProcessedEvent
                    {
                        TransactionId = transactionId,
                        Results = results
                    }, context));
        }
    }
}
