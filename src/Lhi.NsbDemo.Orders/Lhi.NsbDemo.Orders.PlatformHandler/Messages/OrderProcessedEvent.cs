namespace Lhi.NsbDemo.Orders.Messages
{
    public class OrderProcessedEvent
    {
        public string TransactionId { get; set; }
        public string Results { get; set; }
    }
}
