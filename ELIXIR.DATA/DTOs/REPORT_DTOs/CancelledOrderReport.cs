using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class CancelledOrderReport
    {

        public int OrderId { get; set; }
        public string DateOrdered { get; set; }
        public string DateNeeded { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal QuantityOrdered { get; set; }
        public string CancelledDate { get; set; }
        public string Reason { get; set; }
        public string CancelledBy { get; set; }


    }
}
