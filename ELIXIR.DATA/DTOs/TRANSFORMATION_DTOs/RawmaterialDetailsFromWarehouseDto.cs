using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs
{
    public class RawmaterialDetailsFromWarehouseDto
    {

        public int TransformId { get; set; }
        public string Supplier { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public DateTime ManufacturingDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ExpirationDays { get; set; }
        public decimal Balance { get; set; }
        public decimal QuantityNeeded { get; set; }
        public int Batch { get; set; }
        public bool WarehouseItemStatus { get; set; }
        public bool IsPrepared { get; set; }
        public int WarehouseReceivedId { get; set; }
        public decimal PreparationBalance { get; set; }


    }
}
