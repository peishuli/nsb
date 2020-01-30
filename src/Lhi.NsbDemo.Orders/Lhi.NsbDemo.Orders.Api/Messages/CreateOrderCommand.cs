using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lhi.NsbDemo.Orders.Messages
{
    public class CreateOrderCommand
    {
        public string TransactionId { get; set; }
        public string ContractId { get; set; }
        public string ProductId { get; set; }
        public string LocationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserEmail { get; set; }
        public string CustomerName { get; set; }
        public string CustomerId { get; set; }
        public string Street { get; set; }
        public string SuiteOrBuildingN { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public double Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public DateTime DispatchDeliveryDate { get; set; }
        public Dictionary<string, string> CustomOrderDetails { get; set; }
        public string PlatformKey { get; set; }
    }
}
