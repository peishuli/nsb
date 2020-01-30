using System;
using System.Collections.Generic;
using System.Text;

namespace Lhi.NsbDemo.Orders.Messages
{
    public class OrderProcessedEvent
    {
        public string TransactionId { get; set; }
        public string Results { get; set; }
    }
}
