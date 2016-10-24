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
    }
}