using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.TRANSFORMATION_MODEL
{
    public class TransformationPreparation : BaseEntity
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

        [Column(TypeName = "Date")]
        public DateTime ManufacturingDate { 
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime ExpirationDate { 
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityNeeded { 
            get; 
            set;
        }
        public int Batch { 
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal WeighingScale { 
            get;
            set; 
        }
        public bool IsActive {
            get;
            set;
        }
        public DateTime PreparedDate { 
            get; 
            set; 
        }
        public string PreparedBy {
            get; 
            set; 
        }
        public int WarehouseId {
            get;
            set; 
        }
        public bool IsMixed {
            get; 
            set;
        }


    }
}
