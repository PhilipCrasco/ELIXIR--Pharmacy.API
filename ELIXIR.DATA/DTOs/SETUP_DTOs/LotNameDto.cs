using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class LotNameDto
    {
        public int Id { get; set; }
        public string LotNameCode { get; set; }
        public string LotCategory { get; set; }
        public int LotCategoryId { get; set; }

        public string SectionName { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string Reason { get; set; }

    }
}
