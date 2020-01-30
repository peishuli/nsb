using NServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lhi.NsbDemo.Orders.Sagas.Sagas
{
    public class OrdersSagaData : ContainSagaData
    {
        public string CorrelationId { get; set; }
    }
}
