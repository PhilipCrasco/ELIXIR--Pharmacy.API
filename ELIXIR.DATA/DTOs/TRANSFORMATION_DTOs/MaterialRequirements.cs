using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs
{
    public class MaterialRequirements
    {

        public int Id { get; set; }
        public int TransformationId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public int Batch { get; set; }
        public int Version { get; set; }
        public decimal OriginalQuantity { get; set; }
        public decimal Quantity { get; set; }
        public string ProdPlan { get; set; }
        public decimal Reserve { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrepared { get; set; }
        public decimal TotalPrepared { get; set; }
        public bool WarehouseItemStatus { get; set; }
        public int WarehouseId { get; set; }

        public bool RejectStatus { get; set; }
        public string CancelRemarks { get; set; }

    }
}
