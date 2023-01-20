using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs
{
    public class TransformationPlanningDto
    {

        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string ProdPlan { get; set; }
        public int Version { get; set; }
        public int Batch { get; set; }
        public decimal Quantity { get; set; }
        public bool Status { get; set; }
        public string AddedBy { get; set; }
        public string DateAdded { get; set; }
        public bool IsApproved { get; set; }
        public string StatusRemarks { get; set; }
        public bool IsPrepared { get; set; }
        public decimal WarehouseStock { get; set; }
        public int BatchAvailable { get; set; }
        public bool IsMixed { get; set; }
        public int BatchRemaining { get; set; }
        public string RejectRemarks { get; set; }

    }
}
