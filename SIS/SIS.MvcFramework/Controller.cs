using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SIS.HTTP.Cookies.Contracts;
using SIS.MvcFramework.ViewEngine.Contracts;
using SIS.MvcFramework.ViewModel;

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

    public abstract class Controller
    {
        private const string ContentPlaceholder = "@RenderBody";

        protected Controller()
        {
            Response = new HttpResponse { StatusCode = HttpResponseStatusCode.OK };
            var session = HttpSessionStorage.GetSession("tempData");
            if (!session.ContainsParameter("tempData"))
            {
                session.AddParameter("tempData", new Dictionary<string, string>());

            }
            TempData = (Dictionary<string, string>)session.GetParameter("tempData");
        }

        public MvcUserInfo User => GetUserData(this.UserCookieService);

        public MvcUserInfo GetUserData(IUserCookieService cookieService)
        {
            if (!Request.Session.ContainsParameter(".auth_cake"))
            {
                return new MvcUserInfo();
            }

            var sessionParameter = (string)Request.Session.GetParameter(".auth_cake");

            try
            {
                var userName = cookieService.GetUserData(sessionParameter);
                return userName;
            }
            catch (Exception)
            {
                return new MvcUserInfo();
            }
        }

        public IUserCookieService UserCookieService { get; internal set; }

        public IHashService HashService { get; internal set; }

        public IHttpRequest Request { get; set; }

        protected IHttpResponse Response { get; set; }

        public ViewEngine.ViewEngine ViewEngine { get; set; }

        protected Dictionary<string, string> TempData { get; set; }


        private string ProcessFileHtml<T>(string viewName, T model, string layout = GlobalConstants.Layout)
        where T : class
        {

            if ("/".Equals(viewName))
            {
                viewName = "/" + GlobalConstants.HomeIndex;
            }

            var fileName = viewName;

            if (!viewName.StartsWith("/"))
            {
                fileName = "/" + viewName;
            }

            var currentFileName = $"{GlobalConstants.View}{fileName}{GlobalConstants.Html}";

            var fileHtml = System.IO.File.ReadAllText(currentFileName);

            var layoutHtml = System.IO.File.ReadAllText($"{GlobalConstants.View}/{layout}{GlobalConstants.Html}");

            var allContent = layoutHtml.Replace(ContentPlaceholder, fileHtml);

            var cSharpContent = ViewEngine.GetHtml(layout, allContent, model, User);

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

        protected IHttpResponse View<T>(string viewName = null, T model = null, string layout = GlobalConstants.Layout)
            where T : class
        {
            if (viewName == null)
            {
                viewName = this.Request.Path.Trim('/', '\\');
                if (string.IsNullOrWhiteSpace(viewName))
                {
                    viewName = "Home/Index";
                }
            }

            var content = ProcessFileHtml(viewName, model, layout);

            PrepareHtmlResult(content);

            return Response;
        }

        protected IHttpResponse View(string viewName = null, string layout = GlobalConstants.Layout)
        {
            return View(viewName, (object)null, layout);
        }

        protected IHttpResponse View<T>(T model = null, string layout = GlobalConstants.Layout)
            where T : class
        {
            return View(null, model, layout);
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
