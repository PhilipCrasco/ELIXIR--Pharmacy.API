using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class ReasonDto
    {
        public int Id { get; set; }
        public string Menu { get; set; }
        public int MenuId { get; set; }
        public string ReasonName { get; set; }
        public bool IsActive { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
    }
}
