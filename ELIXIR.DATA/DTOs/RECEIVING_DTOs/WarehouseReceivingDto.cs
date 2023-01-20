using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class WarehouseReceivingDto
    {

        public int Id { get; set; }
        public int PO_Number { get; set; }
        public string PO_Date { get; set; }
        public int PR_Number { get; set; }
        public string PR_Date { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Supplier { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal ActualGood { get; set; }
        public decimal Reject { get; set; }
        public string ExpirationDate { get; set; }
        public int ExpirationDay { get; set; }
        public string QC_ReceivedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsWareHouseReceive { get; set; }
        public bool IsExpiryApprove { get; set; }
        public string ManufacturingDate { get; set; }




    }
}
