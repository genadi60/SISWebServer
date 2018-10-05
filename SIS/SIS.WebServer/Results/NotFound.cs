namespace SIS.WebServer.Results
{
    using System;

    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Responses.Contracts;
    
    public class NotFound
    {
        public IHttpResponse PageNotFound()
        {
            string content = string.Format(GlobalConstants.NotFoundPage, DateTime.UtcNow.ToString("R"));

            return new HtmlResult(content, HttpResponseStatusCode.Not_Found);
        }
    }
}
