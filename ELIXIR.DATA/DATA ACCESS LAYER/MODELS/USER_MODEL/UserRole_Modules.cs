using System.ComponentModel.DataAnnotations;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS
{
    public class UserRole_Modules : BaseEntity
    {
        [RegularExpression("^[0-9]*$", ErrorMessage = "NotificationId must be numeric")]
        public int RoleId {
            get; 
            set;
        }

        [RegularExpression("^[0-9]*$", ErrorMessage = "NotificationId must be numeric")]
        public int ModuleId { 
            get;
            set;
        }
        public bool IsActive {
            get; 
            set; 
        }


    }
}
