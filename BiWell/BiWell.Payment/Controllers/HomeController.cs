using BiWell.Payment.Entities;
using System.Globalization;
using System.Web.Mvc;

namespace BiWell.Payment.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View(new PaymentDetails());
        }

        [HttpGet]
        public ActionResult Payment(PaymentDetails paymentDetails)
        {
            if (paymentDetails.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Необходимо указать ненулевую положительную сумму платежа");
            }

            if (ModelState.IsValid)
            {
                string formattedAmount = string.Format(CultureInfo.InvariantCulture, "{0:0.00}", paymentDetails.Amount);
                return Redirect($"http://payment.biwell.ru?orderId={paymentDetails.OrderId}&amount={formattedAmount}");
            }
            else
            {
                return View("Index", paymentDetails);
            }
        }
    }
}