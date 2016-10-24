using System.Web.Mvc;
using System.Web.Routing;

namespace BiWell.Payment
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{orderid}",
                defaults: new { controller = "Home", action = "Index", orderid = UrlParameter.Optional }
            );
        }
    }
}
