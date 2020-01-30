using System;
using System.Collections.Generic;
using System.Text;

namespace Lhi.NsbDemo.Orders.Messages
{
    public class OrderReceivedEvent
    {
        public string TransactionId { get; set; }
    }
}
