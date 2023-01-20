using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class Ordering : BaseEntity
    {

        public int TransactId
        {
            get;
            set;
        }
        public string CustomerName
        {
            get;
            set;
        }
        public string CustomerPosition
        {
            get;
            set;
        }
        public string FarmType
        {
            get;
            set;
        }

        public string FarmCode
        {
            get;
            set;
        }
        public string FarmName
        {
            get;
            set;
        }
        public int OrderNo
        {
            get;
            set;
        }
        public string BatchNo
        {
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime OrderDate
        {
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime DateNeeded
        {
            get;
            set;
        }
        public string TimeNeeded
        {
            get;
            set;
        }
        public string TransactionType
        {
            get;
            set;
        }
        public string ItemCode
        {
            get;
            set;
        }
        public string ItemDescription
        {
            get;
            set;
        }
        public string Uom
        {
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal QuantityOrdered
        {
            get;
            set;
        }
        public string Category
        {
            get;
            set;
        }
        public bool IsActive
        {
            get;
            set;
        }
        public DateTime? PreparedDate
        {
            get;
            set;
        }
        public string PreparedBy
        {
            get;
            set;
        }
        public bool? IsApproved
        {
            get;
            set;
        }
        public DateTime? ApprovedDate
        {
            get;
            set;
        }
        public bool? IsReject
        {
            get;
            set;
        }
        public string RejectBy
        {
            get;
            set;
        }
        public DateTime? RejectedDate
        {
            get;
            set;
        }
        public bool IsPrepared
        {
            get;
            set;
        }

        public bool? IsCancel
        {
            get;
            set;
        }
        public string IsCancelBy
        {
            get;
            set;
        }
        public DateTime? CancelDate
        {
            get;
            set;
        }
        public string Remarks
        {
            get;
            set;
        }

        public int OrderNoPKey
        {
            get;
            set;
        }
        public bool IsMove
        {
            get;
            set;
        }

        public string PlateNumber
        {
            get;
            set;
        }
        public string DeliveryStatus
        {
            get;
            set;
        }

        public DateTime? ReceivedDate
        {
            get;
            set;
        }

    }
}
