namespace CakesWebApp.Controllers
{
    using SIS.HTTP.Responses.Contracts;
    using ViewModels;

    public class HomeController : BaseController
    {
       public HomeController()
        {
           
        }
        public IHttpResponse Index()
        {
            IHttpResponse response = null;

            ViewData["title"] = "The Cake";

            if (IsAuthenticated())
            {
                ViewData["authenticated"] = "bloc";
                ViewData["notAuthenticated"] = "none";
                ViewData["greeting"] = GetUsername();
                ViewData["searchTerm"] = null;
                Request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());

                response = FileViewResponse("/");
            }
            else
            {
                ViewData["authenticated"] = "none";
                ViewData["notAuthenticated"] = "bloc";
                response = FileViewResponse("/");

                if (Request.Cookies.ContainsCookie(".auth_cake"))
                {
                    var cookie = Request.Cookies.GetCookie(".auth_cake");
                    cookie.Delete();
                
                    response.Cookies.Add(cookie);
                }
            }

            return response;
        }

        public IHttpResponse Hello()
        {
            if (!IsAuthenticated())
            {
                ViewData["visible"] = "bloc";
                ViewData["title"] = "Login";
                return FileViewResponse("account/login");
            }

            var userName = GetUsername();
            if (userName == null)
            {
                ViewData["show"] = "none";
            }
            else
            {
                ViewData["show"] = "display";
                ViewData["greeting"] = userName;
            }

            return FileViewResponse("account/hello");
        }

        public IHttpResponse About()
        {
            IsAuthenticated();
            ViewData["title"] = "About Us";
            return FileViewResponse("home/about");
        } 
    }
}
