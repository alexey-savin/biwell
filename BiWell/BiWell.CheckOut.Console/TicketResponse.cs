using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.CheckOut.Console
{
    public class TicketResponse : Response
    {
        public string ticket { get; set; }
        public bool receiverEmailNotRequired { get; set; }
        public bool isCashOnDeliveryOnly { get; set; }
    }
}
