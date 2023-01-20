using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string FarmType { get; set; }
        public int FarmTypeId { get; set; }

        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }
        public string LeadMan { get; set; }
        public string Address { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }

    }
}
