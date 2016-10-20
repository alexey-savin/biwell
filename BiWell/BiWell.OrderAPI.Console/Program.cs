using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.OrderAPI.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ByDesignWebService.Credentials cred = new ByDesignWebService.Credentials();
            cred.Username = "AlexeyInkuev";
            cred.Password = "Moll35$#";
            cred.Token = "token";

            ByDesignWebService.OrderAPISoap cli = new ByDesignWebService.OrderAPISoapClient();
            var versionResponse = cli.Version();

            System.Console.WriteLine("OrderAPI v{0}", versionResponse.Message);

            ByDesignWebService.GetPaymentResponse resp = cli.GetPayments(cred, 1017);
            foreach (var pr in resp.PaymentResponse)
            {
                System.Console.WriteLine(pr.PaymentStatus);
            }
            
                        

            System.Console.ReadKey();
        }
    }
}
