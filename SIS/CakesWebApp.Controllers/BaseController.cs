using Services;
using SIS.HTTP.Requests;

namespace CakesWebApp.Controllers
{
    using System.IO;

    using Data;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;
    
    public abstract class BaseController
    {
        private const string Path = @"C:\Users\genad\OneDrive\Работен плот\C#Web_Basics\September.2018\03.Handmade-Web-Server\SIS\CakesWebApp.Views\";

        protected BaseController()
        {
            Db = new CakesDbContext();
        }

        protected CakesDbContext Db { get; set; }

        protected string GetUserName(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth_cake"))
            {
                return null;
            }

            var cookie = request.Cookies.GetCookie(".auth_cake");

            var userName = UserCookieService.DecryptString(cookie.Value);

            return userName;
        }
        protected IHttpResponse View(string viewName)
        {
            string content = File.ReadAllText(Path + viewName + ".html");

            var response = new HtmlResult(content, HttpResponseStatusCode.OK);

            return response;
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            var response = new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.OK);

            return response;
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            var response = new HtmlResult($"<h1>{errorMessage}</h1>", HttpResponseStatusCode.Internal_Server_Error);

            return response;
        }
    }
}
