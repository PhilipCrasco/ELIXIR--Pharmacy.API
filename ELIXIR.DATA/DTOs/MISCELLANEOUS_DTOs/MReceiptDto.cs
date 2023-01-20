using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.MISCELLANEOUS_DTOs
{
    public class MReceiptDto
    {

        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalQuantity { get; set; }
        public string PreparedDate { get; set; }
        public string AddedBy { get; set; }
        public string Remarks { get; set; }
        public string PreparedBy { get; set; }
        public bool IsActive { get; set; }


    }
}
