namespace CakesWebApp.Controllers
{
    using SIS.HTTP.Responses.Contracts;
    using SIS.MvcFramework.Attributes;
    using ViewModels.Shopping;

    public class HomeController : BaseController
    {
        [HttpGet("/")]
        public IHttpResponse Index()
        {
            ViewData["title"] = "The Cake";

            if (IsAuthenticated())
            {
                ViewData["authenticated"] = "bloc";
                ViewData["notAuthenticated"] = "none";
                ViewData["searchTerm"] = null;
                Request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());

            }
            else
            {
                ViewData["authenticated"] = "none";
                ViewData["notAuthenticated"] = "bloc";
               
                if (Request.Cookies.ContainsCookie(".auth_cake"))
                {
                    var cookie = Request.Cookies.GetCookie(".auth_cake");
                    cookie.Delete();
                
                    Response.AddCookie(cookie);
                }
            }

            return View("/");
        }

        [HttpGet("/hello")]
        public IHttpResponse Hello()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            return View("account/hello");
        }

        [HttpGet("/about")]
        public IHttpResponse About()
        {
            IsAuthenticated();
            ViewData["title"] = "About Us";
            return View("home/about");
        } 
    }
}
