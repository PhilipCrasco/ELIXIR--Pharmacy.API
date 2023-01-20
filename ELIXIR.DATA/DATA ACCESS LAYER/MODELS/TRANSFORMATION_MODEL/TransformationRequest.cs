using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL
{
    public class TransformationRequest : BaseEntity
    {
        public int TransformId { 
            get; 
            set; 
        }
        public string ItemCode {
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
        public DateTime ProdPlan {
            get; 
            set; 
        }

        public bool IsActive {
            get; 
            set;
        }
        public bool IsPrepared { 
            get; 
            set;
        }
           public bool? IsReject { 
            get; 
            set;
        }

        public bool? IsCancelled
        {
            get;
            set;
        }

    }
}
