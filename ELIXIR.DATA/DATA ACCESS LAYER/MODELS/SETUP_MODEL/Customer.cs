using System;
using System.ComponentModel.DataAnnotations;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Customer : BaseEntity
    {
        public string CustomerCode {
            get; 
            set; 
        }
        public string CustomerName { 
            get; 
            set; 
        }
        public FarmType FarmType { 
            get;
            set;
        }
        public int FarmTypeId {
            get; 
            set; 
        }
        public string CompanyName {
            get; 
            set;
        }
        public string MobileNumber { 
            get; 
            set; 
        }
        public string LeadMan {
            get; 
            set; 
        }

        public string Address { 
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
