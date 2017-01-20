using RestSharp;
using System.Xml.Linq;
using static System.Console;

namespace BiWell.Expressrms.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUrl = "http://agregator.postrussia.com";

            var client = new RestClient(baseUrl);
            var request = new RestRequest("api/customerorder", Method.POST);

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("neworder",
                    new XElement("auth",
                        new XAttribute("login", Properties.Settings.Default.ApiLogin),
                        new XAttribute("password", Properties.Settings.Default.ApiPassword)),
                    new XElement("order",
                        new XAttribute("number", "Test BiWell X-01"),
                        new XAttribute("fio", "BiWell test client"),
                        new XAttribute("phone", "9207501122"),
                        new XAttribute("codeProfile", "151"),
                        new XAttribute("zip", ""),
                        new XAttribute("city", "Moscow"),
                        new XAttribute("address", "Krasnaya ploschad 1"),
                        new XAttribute("timeDelivery", ""),
                        new XAttribute("comment", "Testoviy order"),
                        new XAttribute("typeDelivery", "2"),
                        new XAttribute("pointDelivery", "102.1.1"),
                        new XAttribute("receivingPayment", "0"),
                        new XAttribute("assessedValue", "1200.00"),
                        new XAttribute("forPayment", "0.00"),
                        new XAttribute("weight", ""),
                        new XAttribute("seats", "1"),
                        new XElement("goods",
                            new XElement("good",
                                new XAttribute("art", "BW-TEST01"),
                                new XAttribute("name", "Testoviy tovar for dostavka"),
                                new XAttribute("quantity", "3"),
                                new XAttribute("barcode", "1432567"),
                                new XAttribute("weight", "200"),
                                new XAttribute("startingPrice", "125.12"),
                                new XAttribute("totalPrice", "125.12"))))));

            WriteLine(doc.ToString());

            request.AddXmlBody(doc);
            var x = client.Execute(request);
            WriteLine(x.Content);

            ReadKey();
        }
    }
}
