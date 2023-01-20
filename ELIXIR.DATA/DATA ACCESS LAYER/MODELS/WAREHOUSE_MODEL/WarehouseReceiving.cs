using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL
{
    public class WarehouseReceiving : BaseEntity
    {

        public string ItemCode {
            get;
            set;
        }
        public string ItemDescription { 
            get;
            set; 
        }
        public int PO_Number  { 
            get; 
            set; 
        }
        public string Uom { 
            get; 
            set;
        }
        public string Supplier  {
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime ReceivingDate {
            get; 
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime ManufacturingDate {
            get;
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime Expiration { 
            get; 
            set;
        }

        public int ExpirationDays  { 
            get; 
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualDelivered {
            get;
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityGood {
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalReject { 
            get;
            set;
        }

        public string LotCategory {
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualGood { 
            get;
            set;
        }

        public string Reason { 
            get;
            set; 
        }
        public bool IsWarehouseReceive { 
            get;
            set; 
        }
        public bool IsActive { 
            get;
            set; 
        }
        public int QcReceivingId {
            get;
            set;
        }
        public bool ConfirmRejectbyWarehouse { 
            get;
            set;
        }
        public bool ConfirmRejectbyQc {
            get;
            set; 
        }
        public string ReceivedBy {
            get; 
            set;
        }
        public string TransactionType { 
            get;
            set; 
        }

        public int? TransformId { 
            get;
            set;
        }
        public int BatchCount {
            get; 
            set; 
        }

        public int? MiscellaneousReceiptId {
            get; 
            set;
        }

    }
}
