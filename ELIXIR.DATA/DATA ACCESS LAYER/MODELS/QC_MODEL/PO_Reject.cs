using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL
{
    public class PO_Reject : BaseEntity
    { 
        public int PO_ReceivingId {
            get;
            set;
        }
        public int Quantity { 
            get; 
            set;
        }
        public string Remarks {
            get; 
            set; 
        }

    }
}
