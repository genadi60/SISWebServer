namespace CakesWebApp.Controllers
{
    using SIS.HTTP.Responses.Contracts;
    using ViewModels;

    public class HomeController : BaseController
    {
        public IHttpResponse Index()
        {
            ViewData["title"] = "The Cake";

            if (IsAuthenticated())
            {
                ViewData["authenticated"] = "bloc";
                ViewData["notAuthenticated"] = "none";
                ViewData["greeting"] = User;
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

        public IHttpResponse Hello()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return View("account/login");
            }

            var userName = User;
            if (userName == null)
            {
                ViewData["show"] = "none";
            }
            else
            {
                ViewData["show"] = "display";
                ViewData["greeting"] = userName;
            }

            return View("account/hello");
        }

        public IHttpResponse About()
        {
            IsAuthenticated();
            ViewData["title"] = "About Us";
            return View("home/about");
        } 
    }
}
