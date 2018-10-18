namespace SIS.MvcFramework
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using HTTP.Responses.Contracts;
    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using Services;
    using Services.Contracts;
    using WebServer.Results;
    
    public abstract class Controller
    {
        private const string ContentPlaceholder = "{{{content}}}";

        protected Controller()
        {
            UserCookieService = new UserCookieService();
            HashService = new HashService();
            ViewData = new Dictionary<string, string>
            {
                ["visible"] = "bloc"
            };
        }

        protected IUserCookieService UserCookieService { get; set; }

        protected IHashService HashService { get; set; }

        public IHttpRequest Request { get; set; }

        protected Dictionary<string, string> ViewData { get; set; }

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
        
        protected string GetUsername()
        {
            if (!Request.Cookies.ContainsCookie(".auth_cake"))
            {
                return null;
            }

            var cookie = Request.Cookies.GetCookie(".auth_cake");

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

        protected bool IsAuthenticated()
        {
            if(!Request.Cookies.HasCookies() || !Request.Cookies.ContainsCookie(".auth_cake") || !Request.Session.ContainsParameter(".auth_cake"))
            {
                ViewData["authenticated"] = "none";
                ViewData["notAuthenticated"] = "bloc";
                ViewData["visible"] = "bloc";

                return false;
            }
            ViewData["authenticated"] = "bloc";
            ViewData["notAuthenticated"] = "none";
            ViewData["visible"] = "bloc";
            ViewData["greeting"] = GetUsername();
            if (!ViewData.ContainsKey("searchTerm"))
            {
                ViewData["searchTerm"] = null;
            }
            
            return true;
        }

        protected void SetDefaultViewData()
        {
            ViewData["authenticated"] = "none";
        }
    }
}
