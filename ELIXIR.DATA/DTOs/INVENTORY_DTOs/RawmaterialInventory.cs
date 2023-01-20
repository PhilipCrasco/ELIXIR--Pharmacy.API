using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs
{
    public class RawmaterialInventory
    {

     //   public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string LotCategory { get; set; }
        public string Uom { get; set; }
        public decimal SOH { get; set; }
        public decimal Reserve { get; set; }
        public decimal ReceiveIn { get; set; }
        public decimal ReceiveOut { get; set; }
        public bool IsWarehouseReceived { get; set; }


    }
}
