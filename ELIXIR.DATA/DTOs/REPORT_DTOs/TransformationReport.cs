using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class TransformationReport
    {

        public int TransformationId { get; set; }
        public string PlanningDate { get; set; }
        public string ItemCode_Formula { get; set; }
        public string ItemDescription_Formula { get; set; }
        public int Version { get; set; }
        public int Batch { get; set; }
        public decimal Formula_Quantity { get; set; }
        public string ItemCode_Recipe { get; set; }
        public string ItemDescription_Recipe { get; set; }
        public decimal Recipe_Quantity { get; set; }
        public string DateTransformed { get; set; }
        public string DeliveryDate { get; set; }
    }
}
