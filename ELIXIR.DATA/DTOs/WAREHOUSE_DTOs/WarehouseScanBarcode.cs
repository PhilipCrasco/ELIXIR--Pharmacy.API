using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.WAREHOUSE_DTOs
{
    public class WareHouseScanBarcode
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int PO_Number { get; set; }
        public string Uom { get; set; }
        public string Supplier { get; set; }
        public string ManufacturingDate { get; set; }
        public decimal ActualDelivered { get; set; }
        public decimal TotalStock { get; set; }

        public string Expiration { get; set; }
        public int ExpirationDays { get; set; }
        public bool IsActive { get; set; }
        public bool IsWarehouseReceived { get; set; }
        public bool ExpiryIsApprove { get; set; }
        public decimal ExpectedDelivery { get; set; }
        public decimal TotalReject { get; set; }


    }
}
