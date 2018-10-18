namespace CakesWebApp.Controllers
{
    using System.Collections.Generic;

    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using ViewModels;

    public class HomeController : BaseController
    {
        private readonly IHttpRequest _request;

        public HomeController(IHttpRequest request, Dictionary<string,string> viewData) : base(viewData)
        {
            _request = request;
            if (IsAuthenticated(_request))
            {
                ViewData["visible"] = "bloc";
            }
        }
        public IHttpResponse Index()
        {
            IHttpResponse response = null;

            ViewData["title"] = "The Cake";

            if (IsAuthenticated(_request))
            {
                ViewData["authenticated"] = "bloc";
                ViewData["notAuthenticated"] = "none";
                ViewData["greeting"] = GetUsername(_request);
                ViewData["searchTerm"] = null;
                _request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());

                response = FileViewResponse("/");
            }
            else
            {
                ViewData["authenticated"] = "none";
                ViewData["notAuthenticated"] = "bloc";
                response = FileViewResponse("/");

                if (_request.Cookies.ContainsCookie(".auth_cake"))
                {
                    var cookie = _request.Cookies.GetCookie(".auth_cake");
                    cookie.Delete();
                
                    response.Cookies.Add(cookie);
                }
            }

            return response;
        }

        public IHttpResponse Hello()
        {
            if (!IsAuthenticated(_request))
            {
                return FileViewResponse("account/login");
            }

            var userName = GetUsername(_request);
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

        public IHttpResponse About() => FileViewResponse("home/about");
    }
}
