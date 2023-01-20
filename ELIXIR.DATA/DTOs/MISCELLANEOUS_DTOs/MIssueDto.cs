using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.MISCELLANEOUS_DTOs
{
    public class MIssueDto
    {
        public int Id { get; set; }
        public int WarehouseId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string CustomerCode { get; set; }
        public string Customer { get; set; }
        public string PreparedDate { get; set; }
        public string PreparedBy { get; set; }
        public int IssuePKey { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal RemainingStocks { get; set; }
        public string ExpirationDate { get; set; }

        public string Remarks { get; set; }
        public bool IsActive { get; set; }

    }
}
