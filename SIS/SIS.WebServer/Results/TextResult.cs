namespace SIS.WebServer.Results
{
    using System.Text;

    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    public class TextResult : HttpResponse
    {
        public TextResult(string content, HttpResponseStatusCode responseStatusCode, string contentType = "text/plain; charset=utf-8") 
            : base(responseStatusCode)
        {
            Headers.Add(new HttpHeader(GlobalConstants.ContentType, contentType));
            Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
