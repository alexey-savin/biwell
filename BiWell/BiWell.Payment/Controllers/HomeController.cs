using BiWell.Payment.Entities;
using System.Web.Mvc;

namespace BiWell.Payment.Controllers
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View(new PaymentDetails());
        }

        public ActionResult Payment(PaymentDetails paymentDetails)
        {
            if (paymentDetails.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Необходимо указать ненулевую положительную сумму платежа");
            }

            if (ModelState.IsValid)
            {
                return View("Payment", paymentDetails);
            }
            else
            {
                return View("Index", paymentDetails);
            }
        }
    }
}