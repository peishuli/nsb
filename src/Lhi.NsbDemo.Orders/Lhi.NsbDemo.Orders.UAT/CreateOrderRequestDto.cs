using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lhi.NsbDemo.Orders.UAT
{
    public class CreateOrderRequestDto 
    {
        /// <summary>
        /// Guid Identifier for the Contract 
        /// </summary>
        public string ContractId { get; set; }

        /// <summary>
        /// Guid identifier for the Product
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Guid Identifier for location.. may be NULL
        /// </summary>
        public string LocationId { get; set; }

        /// <summary>
        /// First Name of the Contact that placed the Order
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last Name of the Contact that placed the Order
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Phone Number of the Contact that placed the Order
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// email of user placing order
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Customer name
        /// </summary>
        public string CustomerName { get; set; }


        /// <summary>
        /// Guid Id that represents a unique customer. Spans all platforms.
        /// </summary>           
        public string CustomerId { get; set; }


        /// <summary>
        /// Street for the delivery
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Suite or building Number for the delivery
        /// </summary>
        public string SuiteOrBuildingN { get; set; }

        /// <summary>
        /// State for the delivery
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// City for the delivery
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Zip Code for the delivery
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Number of items ordered
        /// </summary>
        public double Quantity { get; set; }
        /// <summary>
        /// LOAD,TON
        /// </summary>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// Date for the delivery dispatch
        /// </summary>
        public DateTime DispatchDeliveryDate { get; set; }

        /// <summary>
        /// List of custom fields specified for each platform
        /// Following are field requirements:
        ///     
        /// </summary>
        public Dictionary<string, string> CustomOrderDetails { get; set; }

        /// <summary>
        /// Platform selector field: 
        ///     "agg" - JWS
        ///     "cement" - CDM
        ///     "rmx" - CMD
        ///     "global" - SAP
        /// </summary>
        public string PlatformKey { get; set; }
    }
}
