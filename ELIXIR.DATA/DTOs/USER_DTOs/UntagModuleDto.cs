using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.USER_DTOs
{
    public class UntagModuleDto
    {
        public string Remarks { get; set; }
        public string MainMenu { get; set; }
        public string SubMenu { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool IsActive { get; set; }



    }
}
