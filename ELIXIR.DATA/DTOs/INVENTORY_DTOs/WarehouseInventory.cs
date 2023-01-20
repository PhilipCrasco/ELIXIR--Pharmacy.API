using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs
{
    public class WarehouseInventory
    {
        public int WarehouseId { get; set; }
        public string ItemCode { get; set; }
        public decimal ActualGood { get; set; }

        public string ExpirationDate { get; set; }
        public int ExpirationDays { get; set; }

    }
}
