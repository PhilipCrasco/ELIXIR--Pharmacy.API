using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL
{
    public class Warehouse_Reject : BaseEntity
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity {
            get;
            set;
        }
        public string Remarks {
            get;
            set; 
        }
        public int WarehouseReceivingId { 
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime RejectByDate {
            get;
            set;
        }

        public bool IsActive { 
            get; 
            set; 
        }
        public string RejectedBy {
            get;
            set; 
        }

    }
}
