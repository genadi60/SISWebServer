using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Drawing;

using SIS.HTTP.Enums;
using SIS.HTTP.Requests;
using SIS.HTTP.Responses;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    using System.Collections.Generic;

    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using ViewModels;
    using static System.Net.Mime.MediaTypeNames;

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
            if (IsAuthenticated(_request))
            {
                ViewData["authenticated"] = "bloc";
                ViewData["notAuthenticated"] = "none";
                ViewData["greeting"] = GetUsername(_request);
                ViewData["searchTerm"] = null;
                _request.Session.AddParameter(ShoppingCartViewModel.SessionKey, new ShoppingCartViewModel());
                return FileViewResponse("home/index");
            }
            else
            {
                ViewData["authenticated"] = "none";
                ViewData["notAuthenticated"] = "bloc";
                if (_request.Cookies.ContainsCookie(".auth_cake"))
                {
                    var cookie = _request.Cookies.GetCookie(".auth_cake");
                    cookie.Delete();
                    response = FileViewResponse("home/index");
                    response.Cookies.Add(cookie);
                }
            }

            
            ViewData["title"] = "The Cake";
            return response;
        }

        public IHttpResponse Hello()
        {
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
