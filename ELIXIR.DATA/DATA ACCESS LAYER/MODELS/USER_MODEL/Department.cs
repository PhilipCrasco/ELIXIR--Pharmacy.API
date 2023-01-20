using System;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS
{
    public class Department : BaseEntity
    {
        public string DepartmentName {
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
        public string Reason {
            get; 
            set; 
        }
    }
}