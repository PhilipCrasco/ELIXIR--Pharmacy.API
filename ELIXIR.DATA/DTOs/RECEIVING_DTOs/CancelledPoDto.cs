using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class CancelledPoDto
    {
        public int Id { get; set; }
        public int PO_Number { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Supplier { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal QuantityCancel { get; set; }
        public decimal QuantityGood { get; set; }
        public string DateCancelled { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }



    }
}
