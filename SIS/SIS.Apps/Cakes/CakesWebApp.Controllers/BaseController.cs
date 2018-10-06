using System.Collections.Generic;
using System.IO;
using System.Linq;
using CakesWebApp.Data;
using CakesWebApp.Services;
using SIS.HTTP.Enums;
using SIS.HTTP.Requests.Contracts;
using SIS.HTTP.Responses.Contracts;
using SIS.WebServer.Results;

namespace CakesWebApp.Controllers
{
    public abstract class BaseController
    {
        private const string ContentPlaceholder = "{{{content}}}";

        protected BaseController()
        {
            Db = new CakesDbContext();
            this.ViewData = new Dictionary<string, string>();
        }
        
        protected Dictionary<string, string> ViewData { get; }

        protected CakesDbContext Db { get; }

        protected IHttpResponse FileViewResponse(string fileName)
        {
            var result = ProcessFileHtml(fileName);

            if (ViewData.Any())
            {
                foreach (var value in ViewData)
                {
                    result = result.Replace($"{{{{{{{value.Key}}}}}}}", value.Value);
                }
            }

            return new HtmlResult(result, HttpResponseStatusCode.OK);
        }

        private string ProcessFileHtml(string fileName)
        {
            var layoutHtml = File.ReadAllText("layout.html");

            var fileHtml = File.ReadAllText(fileName);

            var result = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            return result;
        }
        
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
            var path = viewName + ".html";

            string content = File.ReadAllText(path);

            var response = new HtmlResult(content, HttpResponseStatusCode.OK);

            return response;
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            var response = new HtmlResult($"<h1>{errorMessage}</h1><div><a href=\"/\">Home</a></div>", HttpResponseStatusCode.OK);

            return response;
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            var response = new HtmlResult($"<h1>{errorMessage}</h1><div><a href=\"/\">Home</a></div>", HttpResponseStatusCode.Internal_Server_Error);

            return response;
        }
    }
}
