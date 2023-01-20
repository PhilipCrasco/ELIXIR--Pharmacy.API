using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class FarmDto
    {
        public int Id { get; set; }
        public string FarmCode { get; set; }
        public string FarmName { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
