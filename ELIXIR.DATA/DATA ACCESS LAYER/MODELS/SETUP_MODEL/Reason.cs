using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Reason : BaseEntity
    {
        public int MenuId { 
            get; 
            set;
        }
        public string ReasonName {
            get; 
            set; 
        }
        public bool IsActive {
            get;
            set;
        }
        public DateTime DateAdded { 
            get;
            set; 
        }
        public string AddedBy { 
            get; 
            set;
        }
    }
}
