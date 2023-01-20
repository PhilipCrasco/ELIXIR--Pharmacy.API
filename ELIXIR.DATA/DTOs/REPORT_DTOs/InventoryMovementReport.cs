using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class InventoryMovementReport
    {

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemCategory { get; set; }
        public decimal TotalOut { get; set; }
        public decimal TotalIn { get; set; }
        public decimal Ending { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal PurchasedOrder { get; set; }
        public decimal OthersPlus { get; set; }

    }
}
