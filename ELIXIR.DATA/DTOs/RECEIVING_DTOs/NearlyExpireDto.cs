using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class NearlyExpireDto
    {
        public int Id { get; set; }
        public int PO_Number { get; set; }
        public string PO_Date { get; set; }
        public int PR_Number { get; set; }
        public string PR_Date { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Supplier { get; set; }
        public string Uom { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal ActualGood { get; set; }
        public decimal ActualRemaining { get; set; }
        public string ManufacturingDate { get; set; }
        public string ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int Days { get; set; }
        public bool IsNearlyExpire { get; set; }
        public bool ExpiryIsApprove { get; set; }
        public int ReceivingId { get; set; }
        public string DateOfChecking { get; set; }

        public bool TruckApproval1 { get; set; }
        public bool TruckApproval2 { get; set; }
        public bool TruckApproval3 { get; set; }
        public bool TruckApproval4 { get; set; }

        public string TruckApprovalRemarks1 { get; set; }
        public string TruckApprovalRemarks2 { get; set; }
        public string TruckApprovalRemarks3 { get; set; }
        public string TruckApprovalRemarks4 { get; set; }

        public bool UnloadingApproval1 { get; set; }
        public bool UnloadingApproval2 { get; set; }
        public bool UnloadingApproval3 { get; set; }
        public bool UnloadingApproval4 { get; set; }

        public string UnloadingApprovalRemarks1 { get; set; }
        public string UnloadingApprovalRemarks2 { get; set; }
        public string UnloadingApprovalRemarks3 { get; set; }
        public string UnloadingApprovalRemarks4 { get; set; }

        public bool CheckingApproval1 { get; set; }
        public bool CheckingApproval2 { get; set; }

        public string CheckingApprovalRemarks1 { get; set; }
        public string CheckingApprovalRemarks2 { get; set; }

        public bool QAApproval { get; set; }
        public string QAApprovalRemarks { get; set; }




    }
}
