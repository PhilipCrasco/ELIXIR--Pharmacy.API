using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class TransformationRequirementDto
    {
        public int Id { get; set; }
        public string FormulaCode { get; set; }
        public string FormulaDescription { get; set; }
        public int FormulaVersion { get; set; }
        public decimal FormulaQuantity { get; set; }


        public int RequirementId { get; set; }
        public string RequirementCode { get; set; }
        public string RequirementDescription { get; set; }
        public decimal RequirementQuantity { get; set; }
        public string AddedBy { get; set; }
        public string uom { get; set; }
        public bool IsActive { get; set; }


    }
}
