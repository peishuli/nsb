namespace Lhi.NsbDemo.Orders.Messages
{
    public class SendNotificationCommand
    {
        public string TransactionId { get; set; }
        public string Message { get; set; }
    }
}
