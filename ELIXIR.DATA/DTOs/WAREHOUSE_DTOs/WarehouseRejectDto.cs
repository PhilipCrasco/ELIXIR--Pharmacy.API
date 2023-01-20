using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.WAREHOUSE_DTOs
{
    public class WarehouseRejectDto
    {

        public int warehouseReceivingId { get; set; }
        public string Remarks { get; set; }
        public decimal Quantity { get; set; }
        public string RejectDate { get; set; }

    }
}
