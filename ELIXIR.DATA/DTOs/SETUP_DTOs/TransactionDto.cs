using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class TransactionDto
    {

        public int Id { get; set; }
        public string TransactionName { get; set; }
        public string AddedBy { get; set; }
        public string DateAdded { get; set; }
        public bool IsActive { get; set; }

    }
}
