using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs
{
    public class QcReceivingInventory
    {

        public int Id { get; set; }
        public string ItemCode { get; set; }
        public decimal QcReceive { get; set; }
        public bool IsActive { get; set; }





    }
}
