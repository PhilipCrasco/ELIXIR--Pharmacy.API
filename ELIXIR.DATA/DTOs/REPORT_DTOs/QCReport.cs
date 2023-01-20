using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class QCReport
    {
        public int Id { get; set; }
        public string QcDate { get; set; }
        public int PONumber { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; }
        public string ManufacturingDate { get; set; }
        public string ExpirationDate { get; set; }
        public decimal TotalReject { get; set; }
        public string SupplierName { get; set; }
        public decimal Price { get; set; }
        public string QcBy { get; set; }

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
