namespace SIS.MvcFramework
{
    using System.Collections.Generic;
    using System.Linq;
    
    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Requests.Contracts;
    using HTTP.Responses;
    using HTTP.Responses.Contracts;
    using HTTP.Sessions;
    using Services.Contracts;
    using System.Text;

    public abstract class Controller
    {
        private const string ContentPlaceholder = "{{{content}}}";

        protected Controller()
        {
            Response = new HttpResponse {StatusCode = HttpResponseStatusCode.OK};
            var session = HttpSessionStorage.GetSession("viewData");
            if (!session.ContainsParameter("viewData"))
            {
                session.AddParameter("viewData", new Dictionary<string, string>());
               
            }
            ViewData = (Dictionary<string, string>)session.GetParameter("viewData");
        }

        public IUserCookieService UserCookieService { get; internal set; }

        public IHashService HashService { get; internal set; }

        public IHttpRequest Request { get; set; }

        protected IHttpResponse Response { get; set; }

        protected Dictionary<string, string> ViewData { get; set; }

        protected string User
        {
            get
            {
                if (!Request.Cookies.HasCookies() || !Request.Cookies.ContainsCookie(".auth_cake"))
                {
                    return null;
                }

                var cookie = Request.Cookies.GetCookie(".auth_cake");
                var cookieContent = cookie.Value;
                var userName = UserCookieService.GetUserData(cookieContent);
                return userName;
            }
        }

        private string ProcessFileHtml(string viewName, string layout = GlobalConstants.Layout)
        {
            if ("/".Equals(viewName))
            {
                viewName = GlobalConstants.HomeIndex;
            }
                   
            viewName = $"{GlobalConstants.View}{viewName}{GlobalConstants.Html}";
           
            var layoutHtml = System.IO.File.ReadAllText($"{GlobalConstants.View}{layout}");

            var fileHtml = System.IO.File.ReadAllText(viewName);

            var content = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            if (ViewData.Any())
            {
                foreach (var value in ViewData)
                {
                    content = content.Replace($"{{{{{{{value.Key}}}}}}}", value.Value);
                }
            }

            return content;
        }

        protected IHttpResponse File(byte[] content)
        {

            Response.AddHeader(new HttpHeader(GlobalConstants.ContentDisposition, "inline"));
            Response.StatusCode = HttpResponseStatusCode.OK;
            Response.Content = content;

            return Response;
        }

        protected IHttpResponse Redirect(string location)
        {
            Response.AddHeader(new HttpHeader(GlobalConstants.Location, location));
            Response.StatusCode = HttpResponseStatusCode.See_Other;

            return Response;
        }

        protected IHttpResponse Text(string content)
        {
            Response.AddHeader(new HttpHeader(GlobalConstants.ContentType, "text/plain; charset=utf-8"));
            Response.StatusCode = HttpResponseStatusCode.OK;
            PrepareHtmlResult(content);

            return Response;
        }

        private void PrepareHtmlResult(string content)
        {
            Response.Headers.Add(new HttpHeader(GlobalConstants.ContentType, "text/html; charset=utf-8"));
            Response.Content = Encoding.UTF8.GetBytes(content);
        }

        protected IHttpResponse View(string viewName)
        {
            var content = ProcessFileHtml(viewName);

            PrepareHtmlResult(content);

            return Response;
        }

        protected IHttpResponse BadRequestError(string errorMessage)
        {
            var content = PrepareErrorData(HttpResponseStatusCode.Bad_Request, errorMessage);
            
            PrepareHtmlResult(content);
            
            return Response;
        }

        protected IHttpResponse ServerError(string errorMessage)
        {
            var content = PrepareErrorData(HttpResponseStatusCode.Internal_Server_Error, errorMessage);
            
            PrepareHtmlResult(content);
            
            return Response;
        }

        public IHttpResponse NotFound()
        {
            var content = PrepareErrorData(HttpResponseStatusCode.Not_Found, GlobalConstants.NotFoundPage);
            
            PrepareHtmlResult(content);

            return Response;
        }

        protected bool IsAuthenticated()
        {
            if (!Request.Cookies.HasCookies() || !Request.Cookies.ContainsCookie(".auth_cake") || !Request.Session.ContainsParameter(".auth_cake"))
            {
                ViewData["authenticated"] = "none";
                ViewData["notAuthenticated"] = "bloc";
                ViewData["visible"] = "bloc";

                return false;
            }
            ViewData["authenticated"] = "bloc";
            ViewData["notAuthenticated"] = "none";
            ViewData["visible"] = "bloc";
            ViewData["cart"] = "bloc";
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

        private string PrepareErrorData(HttpResponseStatusCode statusCode, string errorMessage)
        {
            ViewData["errorMessage"] = errorMessage;
            ViewData["title"] = "Error";
            ViewData["visible"] = "none";

            var content = ProcessFileHtml("error");
            Response.StatusCode = statusCode;
            return content;
        }
    }
}
