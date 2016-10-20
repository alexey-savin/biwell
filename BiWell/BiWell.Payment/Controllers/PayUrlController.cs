using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BiWell.Payment.Controllers
{
    public class PayUrlController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage PaymentNotification()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("SUCCESS", Encoding.UTF8, "text/plain");

            return response;
        }

        [HttpGet]
        public HttpResponseMessage Echo()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent("BiWell Web Api", Encoding.UTF8, "text/plain");

            return response;
        }
    }
}
