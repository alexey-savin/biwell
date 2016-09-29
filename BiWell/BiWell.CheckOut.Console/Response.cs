using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.CheckOut.Console
{
    public class Response
    {
        public bool error { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}
