namespace SIS.MvcFramework
{
    using System.Collections.Generic;
    using System.Text;

    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Requests.Contracts;
    using HTTP.Responses;
    using HTTP.Responses.Contracts;
    using HTTP.Sessions;
    using Services.Contracts;
    using ViewModels;

    public abstract class Controller
    {
        private const string ContentPlaceholder = "@RenderBody";

        protected Controller()
        {
            Response = new HttpResponse {StatusCode = HttpResponseStatusCode.OK};
            var session = HttpSessionStorage.GetSession("tempData");
            if (!session.ContainsParameter("tempData"))
            {
                session.AddParameter("tempData", new Dictionary<string, string>());
               
            }
            TempData = (Dictionary<string, string>)session.GetParameter("tempData");
        }

        public IUserCookieService UserCookieService { get; internal set; }

        public IHashService HashService { get; internal set; }

        public IHttpRequest Request { get; set; }

        protected IHttpResponse Response { get; set; }

        public ViewEngine.ViewEngine ViewEngine { get; set; }

        protected Dictionary<string, string> TempData { get; set; }

        protected string User
        {
            get
            {
                string userName = null;

                if (!Request.Session.ContainsParameter(".auth_cake") && !Request.Cookies.ContainsCookie(".auth_cake"))
                {
                    return null;
                }

                if (Request.Session.ContainsParameter(".auth_cake"))
                {
                    userName = UserCookieService.GetUserData(Request.Session.GetParameter(".auth_cake").ToString());
                }
                
                return userName;
            }
        }

        private string ProcessFileHtml<T>(string viewName, T model,  string layout = GlobalConstants.Layout)
        {
            if ("/".Equals(viewName))
            {
                viewName = "/" + GlobalConstants.HomeIndex;
            }
                   
            var fileName = $"{GlobalConstants.View}{viewName}{GlobalConstants.Html}";

            var fileHtml = System.IO.File.ReadAllText(fileName);

            var layoutHtml = System.IO.File.ReadAllText($"{GlobalConstants.View}/{layout}{GlobalConstants.Html}");

            layoutHtml = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            var cSharpContent = ViewEngine.GetHtml(viewName, layoutHtml, model, User);

            return cSharpContent;
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

        protected IHttpResponse View<T>(string viewName, T model)
        {
            if (!viewName.StartsWith("/"))
            {
                viewName = "/" + viewName;
            }
            
            var content = ProcessFileHtml(viewName, model);

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
        
        protected void SetDefaultTempData()
        {
            TempData["searchTerm"] = null;
        }

        private string PrepareErrorData(HttpResponseStatusCode statusCode, string errorMessage)
        {
            var model = new ErrorViewModel(errorMessage);
            var content = ProcessFileHtml("/error", model);
            Response.StatusCode = statusCode;
            return content;
        }
    }
}
