using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs
{
    public class TransformationWithRequirements
    {

        public int Id { get; set; }
        public int TransformationId { get; set; }
        public string FormulaCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public int Batch { get; set; }
        public int Version { get; set; }
        public decimal Quantity { get; set; }
        public string ProdPlan { get; set; }
        public bool IsActive { get; set; }
        public decimal QuantityNeeded { get; set; }
        public decimal WarehouseStock { get; set; }



    }
}
