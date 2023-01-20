using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.USER_DTOs
{
    public class RolewithModulesDto
    {
        public string RoleName { get; set; }
        public string MainMenu { get; set; }
        public int MainMenuId { get; set; }
        public string SubMenu { get; set; }
        public string ModuleName { get; set; }
        public string MenuPath { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string ModuleStatus { get; set; }



    }
}
