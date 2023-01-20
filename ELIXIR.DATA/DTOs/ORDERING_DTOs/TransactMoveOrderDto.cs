using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class TransactMoveOrderDto
    {

        public int Id { get; set; }
        public string OrderDate { get; set; }
        public int OrderNo { get; set; }
        public string DateNeeded { get; set; }
        public string Farm { get; set; }
        public string FarmCode { get; set; }
        public string FarmType { get; set; }
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal TotalQuantityOrder { get; set; }
        public string PreparedDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrepared { get; set; }
        public bool IsMove { get; set; }


    }
}
