 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL
{
    public class TransformationPlanning : BaseEntity
    {
        public string ItemCode { 
            get; 
            set;
        }
        public string ItemDescription {
            get;
            set;
        }
        public string Uom {
            get; 
            set; 
        }
        public DateTime ProdPlan {
            get; 
            set;
        }
        public int Version { 
            get; 
            set; 
        }
        public int Batch { 
            get; 
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { 
            get;
            set; 
        }
        public bool Status {
            get;
            set; 
        }
        public string AddedBy {
            get; 
            set;
        }
        public DateTime DateAdded {
            get; 
            set; 
        }

        public DateTime? DateApproved {
            get; 
            set; 
        }
        public bool? Is_Approved {
            get; 
            set;
        }

        public string RejectedBy { 
            get; 
            set; 
        }
        public DateTime? RejectedDate {
            get;
            set; 
        }
        public bool IsPrepared { 
            get; 
            set;
        }

        public string CancelRemarks { 
            get; 
            set; 
        }
        public string RejectRemarks {
            get; 
            set;
        }

        public string StatusRequest {
            get; 
            set;
        }
        public bool? IsMixed {
            get;
            set; 
        }




    }
}
