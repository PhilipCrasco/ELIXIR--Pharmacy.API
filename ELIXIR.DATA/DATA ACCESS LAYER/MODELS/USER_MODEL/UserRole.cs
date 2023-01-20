using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS
{
    public class UserRole : BaseEntity
    {
        public string RoleName {
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
        public string ModifiedBy {
            get;
            set;
        }
        public DateTime DateModified { 
            get; 
            set;
        }
        public string Reason { 
            get; 
            set;
        }

    }
}