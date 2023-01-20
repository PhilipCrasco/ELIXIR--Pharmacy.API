using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class MoveOrderReport
    {
        public int MoveOrderId { get; set; }
        public int OrderNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string FarmType { get; set; }
        public string FarmCode { get; set; }
        public string FarmName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; }
        public string ExpirationDate { get; set; }
        public string TransactionType { get; set; }
        public string MoveOrderBy { get; set; }
        public string MoveOrderDate { get; set; }
        public string TransactedDate { get; set; }
        public string DateNeeded { get; set; }
        public string TransactedBy { get; set; }
        public string BatchNo { get; set; }
        public string DeliveryDate { get; set; }


    }
}
