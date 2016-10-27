using BiWell.Payment.Interfaces;
using BiWell.Payment.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace BiWell.Payment.Controllers
{
    public class PayUrlController : ApiController
    {
        private IPaymentNotificationProcessor _processor = null;

        public PayUrlController(IPaymentNotificationProcessor processor)
        {
            _processor = processor;
        }

        [HttpPost]
        public HttpResponseMessage PaymentNotification(PaymentNotificationData data)
        {
            string content = "FAIL";

            if (data != null && !string.IsNullOrEmpty(data.mnt_id) && !string.IsNullOrEmpty(data.mnt_transaction_id))
            {
                content = "SUCCESS";
            }

            if (_processor != null)
            {
                _processor.ProcessNotification(data);
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(content, Encoding.UTF8, "text/plain");

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
