namespace SIS.Demo.Controllers
{
    using System;

    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Responses.Contracts;
    using WebServer.Results;

    public class HomeController
    {
        public IHttpResponse Index()
        {
            var content = string.Format(GlobalConstants.GreetingString, DateTime.UtcNow.ToString("R"));

            var response = new HtmlResult(content, HttpResponseStatusCode.OK);

            response.Cookies.Add("lang", "en");
            
            return response;
        }
    }
}
