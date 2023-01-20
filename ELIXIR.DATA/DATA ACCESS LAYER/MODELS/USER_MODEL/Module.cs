using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;
using System;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS
{
    public class Module : BaseEntity
    {
        public MainMenu MainMenu { 
            get; 
            set;
        }
        public int MainMenuId { 
            get;
            set;
        }
        public string SubMenuName { 
            get; 
            set;
        }
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
        public string ModuleStatus { 
            get;
            set; 
        }
   
    }
}
