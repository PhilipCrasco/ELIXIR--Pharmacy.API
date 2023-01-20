using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL
{
    public class TransformationReject : BaseEntity
    {

        public int TransformId {
            get; 
            set;
        }
        public string FormulaCode { 
            get;
            set;
        }
        public string FormulaDescription {
            get; 
            set; 
        }
        public string Uom { 
            get; 
            set; 
        }
        public int Batch { 
            get;
            set; 
        }
        public int Version {
            get; 
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { 
            get; 
            set;
        }

        public string RawmaterialCode {
            get; 
            set;
        }
        public string RawmaterialDescription { 
            get;
            set;
        }

        public DateTime ProdPlan {
            get; 
            set; 
        }

        public DateTime RejectedDate {
            get; 
            set; 
        }
        public string RejectedBy { 
            get; 
            set;
        }

        public bool IsActive {
            get;
            set; 
        }

        public string RejectRemarks
        {
            get;
            set;
        }


    }
}
