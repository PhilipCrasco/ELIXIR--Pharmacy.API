using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public bool IsActive { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public string Reason { get; set; }
    }
}
