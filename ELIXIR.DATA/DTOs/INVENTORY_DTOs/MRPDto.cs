using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs
{
    public class MRPDto
    {

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemCategory { get; set; }
        public string Uom { get; set; }
        public decimal Price { get; set; }
        public decimal SOH { get; set; }
        public decimal Reserve { get; set; }
        public decimal BufferLevel { get; set; }
        public decimal ReceiveIn { get; set; }
        public decimal ReceiptIn { get; set; }
        public decimal MoveOrderOut { get; set; }
        public decimal IssueOut { get; set; }
        public decimal QCReceiving { get; set; }
        public decimal TotalPrice { get; set; }

        public decimal SuggestedPo { get; set; }
        public decimal AverageIssuance { get; set; }
        public decimal DaysLevel { get; set; }

        public string LastUsed { get; set; }

        public decimal ReserveUsage { get; set; }

        public decimal TransformFrom { get; set; }
        public decimal TransformTo { get; set; }






    }
}
