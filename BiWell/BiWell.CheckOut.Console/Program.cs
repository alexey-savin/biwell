using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BiWell.CheckOut.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://platform.checkout.ru";

            //
            var client = new RestClient(baseUrl);
            var request = new RestRequest("service/login/ticket/{apiKey}", Method.GET);
            request.AddUrlSegment("apiKey", Properties.Settings.Default.ApiKey);

            IRestResponse<TicketResponse> response = client.Execute<TicketResponse>(request);
            TicketResponse tr = response.Data;

            bool isValidSession = false;
            if (!tr.error)
            {
                System.Console.WriteLine("Ticket = " + tr.ticket);
                isValidSession = true;
            }
            else
            {
                System.Console.WriteLine("Ticket error: " + tr.errorMessage);
            }

            if (isValidSession)
            {
                request = new RestRequest();
                request.Method = Method.GET;
                request.Resource = "service/checkout/getPlaceByPostalCode";

                System.Console.Write("Index: ");
                string index = System.Console.ReadLine();

                request.AddParameter("ticket", tr.ticket);
                request.AddParameter("postIndex", index);

                var r = client.Execute<SuggestionResponse>(request);
                SuggestionResponse sr = r.Data;

                if (!sr.error)
                {
                    System.Console.WriteLine("Address = " + sr.fullName);
                }
                else
                {
                    System.Console.WriteLine("Place by postal code error: " + sr.errorMessage);
                }
            }

            System.Console.ReadKey();
        }

         
    }
}
