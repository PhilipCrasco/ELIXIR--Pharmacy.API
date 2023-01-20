using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs
{
    public class TransformationInventory
    {

        public int WarehouseId { get; set; }
        public string ItemCode { get; set; }
        public decimal WeighingScale { get; set; }


    }
}
