using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs
{
    public class TransformationMixingRequirements
    {

        public int TransformId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public int Batch { get; set; }
        public decimal QuantityBatch { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal WeighingScale { get; set; }


    }
}
