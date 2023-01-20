using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL
{
    public class PO_Receiving : BaseEntity
    {

        public int PO_Summary_Id {
            get; 
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime Manufacturing_Date {
            get; 
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public int Expected_Delivery {
            get; 
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime Expiry_Date {
            get; 
            set; 
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Actual_Delivered {
            get; 
            set;
        }
        public string ItemCode {
            get;
            set; 
        }
        public string Batch_No
        {
            get;
            set;
        }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalReject {
            get;
            set;
        }
        public bool IsActive {
            get; 
            set; 
        }

        [Column(TypeName = "Date")]
        public DateTime? CancelDate {
            get;
            set; 
        }
        public string CancelBy {
            get; 
            set; 
        }
        public string Reason { 
            get;
            set; 
        }

        public bool Truck_Approval1 {
            get; 
            set; 
        }

        public bool Truck_Approval2 {
            get; 
            set;
        }
        public bool Truck_Approval3 {
            get;
            set; 
        }
        public bool Truck_Approval4 { 
            get; 
            set;
        }
        public string Truck_Approval1_Remarks { 
            get;
            set;
        }
        public string Truck_Approval2_Remarks {
            get;
            set; 
        }
        public string Truck_Approval3_Remarks {
            get;
            set; 
        }
        public string Truck_Approval4_Remarks {
            get;
            set; 
        }

        public bool Unloading_Approval1 { 
            get; 
            set; 
        }
        public bool Unloading_Approval2 { 
            get; 
            set;
        }
        public bool Unloading_Approval3 {
            get; 
            set;
        }
        public bool Unloading_Approval4 {
            get;
            set;
        }
        public string Unloading_Approval1_Remarks {
            get; 
            set;
        }
        public string Unloading_Approval2_Remarks { 
            get; 
            set; 
        }
        public string Unloading_Approval3_Remarks { 
            get; 
            set; 
        }
        public string Unloading_Approval4_Remarks { 
            get;
            set; 
        }

        public bool Checking_Approval1 { 
            get;
            set;
        }

        public bool Checking_Approval2 {
            get; 
            set; 
        }

        public string Checking_Approval1_Remarks { 
            get; 
            set; 
        }

        public string Checking_Approval2_Remarks { 
            get; 
            set;
        }

        public bool QA_Approval { 
            get;
            set; 
        }

        public string QA_Approval_Remarks {
            get;
            set;
        }

        public bool? ExpiryIsApprove {
            get; 
            set;
        }

        public bool? IsNearlyExpire { 
            get; 
            set;
        }

        public string ExpiryApproveBy {
            get;
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime? ExpiryDateOfApprove { 
            get; 
            set;
        }

        [Column(TypeName = "Date")]
        public DateTime QC_ReceiveDate {
            get; 
            set; 
        }

        public bool ConfirmRejectByQc { 
            get;
            set;
        }
        public bool? IsWareHouseReceive {
            get; 
            set;
        }
        public string CancelRemarks {
            get;
            set; 
        }
        public string QcBy {
            get; 
            set;
        }


    }
}
