using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class Transaction : BaseEntity
    {

        public string TransactionName { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsActive { get; set; }
      


    }
}
