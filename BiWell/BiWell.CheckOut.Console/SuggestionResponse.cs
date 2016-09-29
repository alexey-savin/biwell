using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.CheckOut.Console
{
    public class SuggestionResponse : Response
    {
        public string id { get; set; }
        public string name { get; set; }
        public string fullName { get; set; }
    }
}
