using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lhi.NsbDemo.Orders.Api.Models
{
    /// <summary>
    /// Generic Object Return to encapsulate the new order 
    /// </summary>
    public class CreateOrderResponseDto 
    {
        /// <summary>
        /// New Order ID Dispatch
        /// </summary>
        public Int64 DispatchNo { get; set; }
        /// <summary>
        /// Error Code if Exist
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// User whoe request the Order
        /// </summary>
        public string UserEmail { get; set; }
        /// <summary>
        /// Email from the agent to review and process the order
        /// </summary>
        public string OrderDeskEmail { get; set; }
        /// <summary>
        /// Contract Description
        /// </summary>
        public string SalesOrderName { get; set; }
        /// <summary>
        /// Product Description
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Result Code processing the order
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// Company Name of Customer who owns the Contract related 
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Customer ID from Platform 
        /// </summary>
        public string CustomerID { get; set; }
       
    }
}
