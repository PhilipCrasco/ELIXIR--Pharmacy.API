using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL
{
    public  class MainMenu : BaseEntity
    {
        public string ModuleName { 
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
        public bool IsActive { 
            get;
            set; 
        }
        public string ModifiedBy { 
            get;
            set;
        }
        public string Reason {
            get; 
            set;
        }
        public string MenuPath {
            get;
            set;
        }

    }
}
