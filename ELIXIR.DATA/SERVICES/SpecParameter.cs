using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.SERVICES
{
    public class SpecParameter
    {
        private string _search;
        private string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }
    }
}
