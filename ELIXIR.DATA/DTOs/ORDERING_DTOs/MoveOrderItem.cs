using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class MoveOrderItem
    {

        public int OrderNo { get; set; }
        public int OrderPKey { get; set; }
        public decimal QuantityPrepared { get; set; }
        public bool IsActive { get; set; }


    }
}
