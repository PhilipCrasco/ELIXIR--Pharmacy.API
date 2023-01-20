using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS
{
    public class User : BaseEntity
    {
 
        [StringLength(50)]
        public string FullName { 
            get; 
            set;
        }
     
        [StringLength(25, ErrorMessage = " Must be between 3 and 25 characters!", MinimumLength = 3)]
        public string UserName {
            get;
            set;
        }

        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters!", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password {
            get; 
            set; 
        }
        public bool IsActive {
            get;
            set;
        }
        public UserRole UserRole { 
            get;
            set;
        }

        [RegularExpression("^[0-9]*$", ErrorMessage = "UserRoleId must be numeric")]
        public  int UserRoleId {
            get; 
            set;
        }

        public Department Department {
            get;
            set;
        }

        [RegularExpression("^[0-9]*$", ErrorMessage = "DepartmentId must be numeric")]
        public int DepartmentId { 
            get; 
            set; 
        }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
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
      
        public string Reason {
            get; 
            set; 
        }
    }

}
