using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs
{
    public class TransformationPreparationDto
    {

        public int TranformationId { get; set; }
        public string FormulaCode { get; set; }
        public string FormulaDescription { get; set; }
        public decimal FormulaQuantity { get; set; }


        public string RawmaterialCode { get; set; }
        public string RawmaterialDescription { get; set; }
        public string Uom { get; set; }
        public int Batch { get; set; }
        public decimal RawmaterialQuantity { get; set; }
        public bool IsRequirementActive { get; set; }
        public bool IsPrepared { get; set; }




    }
}
