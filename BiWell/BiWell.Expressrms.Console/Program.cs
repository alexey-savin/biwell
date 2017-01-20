using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using System.Xml;
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

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("root",
                    new XElement("auth",
                        new XAttribute("login", "biwellrus"),
                        new XAttribute("password", "ppV5pIhSRT")),
                    new XElement("orders",
                        new XElement("order",
                            new XAttribute("number", "X02"),
                            new XAttribute("deliveryPlannedMoment", "2017-01-25"),
                            new XAttribute("fio", "BiWell test client"),
                            new XAttribute("phone", "9207501122"),
                            new XAttribute("phone2", ""),
                            new XAttribute("email", "test_my_order@email.com"),
                            new XAttribute("codeProfile", "80"),
                            new XAttribute("zip", "112125"),
                            new XAttribute("city", "Moscow"),
                            new XAttribute("address", "Krasnaya ploschad 1"),
                            new XAttribute("timeDelivery", "10:00 - 18:00"),
                            new XAttribute("comment", "Testoviy order"),
                            new XAttribute("description", "test my order"),
                            new XAttribute("typeDelivery", "2"),
                            new XAttribute("pointDelivery", "301.2.1"),
                            new XAttribute("receivingPayment", "0"),
                            new XAttribute("assessedValue", "1200.00"),
                            new XAttribute("forPayment", "0.00"),
                            new XAttribute("weight", ""),
                            new XAttribute("seats", "1"),
                            new XAttribute("deliveryPrice", "45.95"),
                            new XElement("goods",
                                new XElement("good",
                                    new XAttribute("art", "BW-TEST01"),
                                    new XAttribute("name", "Testoviy tovar for dostavka"),
                                    new XAttribute("quantity", "3"),
                                    new XAttribute("barcode", "1432567"),
                                    new XAttribute("weight", "200"),
                                    new XAttribute("startingPrice", "125.12"),
                                    new XAttribute("totalPrice", "125.12")))))));

            WriteLine(doc.ToString());

            XmlDocument docXml = new XmlDocument();
            docXml.LoadXml(doc.ToString());

            var request = new RestRequest("api/customerorder", Method.POST);
            request.RequestFormat = DataFormat.Xml;
            request.XmlSerializer = new DotNetXmlSerializer();
            request.AddXmlBody(docXml);

            var x = client.Execute(request);

            WriteLine();
            WriteLine("server response:");
            WriteLine(x.Content);

            ReadKey();
        }
    }
}
