namespace CakesWebApp.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Data;
    using Services;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Requests.Contracts;
    using SIS.HTTP.Responses.Contracts;
    using SIS.WebServer.Results;

    public abstract class BaseController
    {
        private const string ContentPlaceholder = "{{{content}}}";

        protected BaseController(Dictionary<string,string> viewData)
        {
            Db = new CakesDbContext();
            ViewData = viewData;
            ViewData["visible"] = "bloc";
        }
        
        protected Dictionary<string, string> ViewData { get; }

        protected CakesDbContext Db { get; }

        protected IHttpResponse FileViewResponse(string fileName)
        {
            if ("/".Equals(fileName))
            {
                fileName = "home/index.html";
            }
            else
            {
                fileName = fileName + ".html";
            }
            

            var result = ProcessFileHtml(fileName);

            return new HtmlResult(result, HttpResponseStatusCode.OK);
        }

        private string ProcessFileHtml(string fileName)
        {
            var layoutHtml = File.ReadAllText("_layout.html");

            var fileHtml = File.ReadAllText(fileName);

            var result = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            if (ViewData.Any())
            {
                foreach (var value in ViewData)
                {
                    result = result.Replace($"{{{{{{{value.Key}}}}}}}", value.Value);
                }
            }

            return result;
        }
        
        protected string GetUsername(IHttpRequest request)
        {
            if (!request.Cookies.ContainsCookie(".auth_cake"))
            {
                return null;
            }

            var cookie = request.Cookies.GetCookie(".auth_cake");

            var username = UserCookieService.DecryptString(cookie.Value);

            return username;
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
            ViewData["errorMessage"] = errorMessage;
            ViewData["title"] = "Error";
            ViewData["visible"] = "none";
            return FileViewResponse("error");
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            ViewData["errorMessage"] = errorMessage;

            var content = ProcessFileHtml("error");
            
            var response = new HtmlResult(content, HttpResponseStatusCode.Internal_Server_Error);

            return response;
        }

        protected void SetDefaultViewData()
        {
            ViewData["authenticated"] = "none";
        }

        protected bool IsAuthenticated(IHttpRequest request)
        {
            if(!request.Cookies.HasCookies() || !request.Cookies.ContainsCookie(".auth_cake") || !request.Session.ContainsParameter(".auth_cake"))
            {
                ViewData["authenticated"] = "none";
                ViewData["notAuthenticated"] = "bloc";
                ViewData["visible"] = "bloc";

                return false;
            }
            ViewData["authenticated"] = "bloc";
            ViewData["notAuthenticated"] = "none";
            ViewData["visible"] = "bloc";
            ViewData["greeting"] = GetUsername(request);
            if (!ViewData.ContainsKey("searchTerm"))
            {
                ViewData["searchTerm"] = null;
            }
            
            return true;
        }
    }
}
