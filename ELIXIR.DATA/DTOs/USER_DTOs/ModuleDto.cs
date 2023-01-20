using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs
{
    public class ModuleDto
    {
        public int Id { get; set; }
        public string MainMenu { get; set; }
        public int MainMenuId { get; set; }
        public string ModuleName { get; set; }
        public string SubMenuName { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public string Reason { get; set; }
        public string ModuleStatus { get; set; }


    }
}
