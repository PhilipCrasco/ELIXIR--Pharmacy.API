using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class UomDto
    {
        public int Id { get; set; }
        public string UOM_Code { get; set; }
        public string UOM_Description { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string Reason { get; set; }


    }
}
