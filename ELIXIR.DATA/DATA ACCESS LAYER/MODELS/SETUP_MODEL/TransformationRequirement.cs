using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class TransformationRequirement : BaseEntity
    {
        public TransformationFormula TransformationFormula {
            get; set;
        }
        public int TransformationFormulaId { 
            get; 
            set;
        }
        public RawMaterial RawMaterial { 
            get;
            set;
        }
        public int RawMaterialId { 
            get;
            set; 
        }
        public string ItemDescription {
            get; 
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity {
            get; 
            set; 
        }
        public string AddedBy {
            get;
            set; 
        }
        public bool IsActive { 
            get; 
            set; 
        }

    }
}
