using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.IMPORT_MODEL
{
    public class ImportPOSummary : BaseEntity
    {
        
        public int PR_Number {
            get; 
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime PR_Date { 
            get; 
            set;
        }

        public int PO_Number {
            get; 
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime PO_Date {
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
        public decimal Ordered { 
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Delivered {
            get;
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Billed {
            get; 
            set;
        }

        public string UOM { 
            get; 
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice {
            get;
            set;
        }

        public string VendorName { 
            get; 
            set;
        }
        public bool IsActive {
            get; 
            set;
        }
        public string Reason {
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public Nullable<DateTime> Date_Cancellation { 
            get; 
            set;
        }

        public DateTime ImportDate { 
            get; 
            set;
        }
        public string AddedBy { 
            get;
            set; 
        }

    }
}
