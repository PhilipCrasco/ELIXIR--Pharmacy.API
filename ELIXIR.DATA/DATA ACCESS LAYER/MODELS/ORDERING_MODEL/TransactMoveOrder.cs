using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
   public class TransactMoveOrder : BaseEntity
    {

        public int OrderNo {
            get; 
            set;
        }
        public string FarmType {
            get; 
            set;
        }
        public string FarmCode {
            get; 
            set; 
        }
        public string FarmName { 
            get; 
            set; 
        }
        public bool IsActive {
            get;
            set; 
        }
        public bool? IsApprove {
            get; 
            set;
        }
        public string PreparedBy { 
            get; 
            set;
        }
        public DateTime? PreparedDate {
            get;
            set; 
        }
        public int OrderNoPKey {
            get;
            set; 
        }
        public DateTime? DeliveryDate { 
            get; 
            set; 
        }
        public bool IsTransact { 
            get; 
            set;
        }


    }
}
