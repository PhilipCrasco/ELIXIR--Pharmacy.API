using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.WAREHOUSE_DTOs
{
    public class WarehouseReceived
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int PO_Number { get; set; }
        public string Uom { get; set; }
        public string Supplier { get; set; }
        public string ManufacturingDate { get; set; }
        public string ReceivedDate { get; set; }
        public decimal TotalStock { get; set; }
        public decimal Quantity { get; set; }
        public bool IsActive { get; set; }


    }
}
