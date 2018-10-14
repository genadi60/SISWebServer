namespace CakesWebApp.Controllers
{
    using System.Collections.Generic;

    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using ViewModels;

    public class HomeController : BaseController
    {
        public HomeController(Dictionary<string,string> viewData) : base(viewData)
        {
            
        }
        public IHttpResponse Index(IHttpRequest request)
        {
            if (request.Cookies.HasCookies() && request.Cookies.ContainsCookie(".auth_cake"))
            {
                ViewData["authenticated"] = "bloc";
                ViewData["notAuthenticated"] = "none";
                ViewData["greeting"] = GetUserName(request);
                ViewData["searchTerm"] = null;
                request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());
            }
            else
            {
                ViewData["authenticated"] = "none";
                ViewData["notAuthenticated"] = "bloc";
            }

            ViewData["title"] = "The Cake";
            return FileViewResponse("home/index");
        }

        public IHttpResponse Hello(IHttpRequest request)
        {
            var userName = GetUserName(request);
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

        public IHttpResponse About(IHttpRequest request) => FileViewResponse("home/about");

        public IHttpResponse Favicon(IHttpRequest request) => FileViewResponse("favicon");

    }
}
