using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class MoveOrder : BaseEntity
    {
        public int OrderNo { 
            get; 
            set;
        }
        public string FarmType { 
            get;
            set; 
        }
        public string FarmCode { 
            get;
            set; 
        }
        public string FarmName {
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
        public string Uom {
            get; 
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityOrdered {
            get; 
            set;
        }
        public string Category {
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime OrderDate {
            get; 
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime DateNeeded { 
            get; 
            set;
        }

        public int WarehouseId { 
            get;
            set; 
        }
        public bool IsActive { 
            get; 
            set; 
        }
        public bool? IsApprove { 
            get;
            set; 
        }

        public DateTime? ApprovedDate {
            get; 
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime? ApproveDateTempo { 
            get;
            set;
        }
        
        public bool IsPrepared { 
            get; 
            set; 
        }
        public string PreparedBy { 
            get; 
            set;
        }
        public DateTime? PreparedDate {
            get; 
            set; 
        }

        public bool? IsCancel {
            get; 
            set; 
        }
        public string CancelBy { 
            get;
            set; 
        }
        public DateTime? CancelledDate {
            get;
            set; 
        }

        public int OrderNoPKey { 
            get; 
            set; 
        }
        public DateTime? ExpirationDate {
            get; 
            set;
        }
        public string PlateNumber {
            get; 
            set; 
        }
        public string DeliveryStatus {
            get; 
            set;
        }
        public bool? IsReject {
            get; 
            set; 
        }
        public string RejectBy {
            get;
            set; 
        }
        public DateTime? RejectedDate {
            get; 
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime? RejectedDateTempo { 
            get; 
            set; 
        }

        public string Remarks { 
            get; 
            set;
        }

        public bool IsTransact {
            get; 
            set; 
        }
        public bool? IsPrint { 
            get;
            set;
        }

        public bool? IsApproveReject
        {
            get;
            set;
        }

        public string BatchNo
        {
            get;
            set;
        }

    }
}
