namespace SIS.WebServer.Results
{
    using System.Text;

    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    public class TextResult : HttpResponse
    {
        public TextResult(string content, HttpResponseStatusCode responseStatusCode) : base(responseStatusCode)
        {
            Headers.Add(new HttpHeader(GlobalConstants.ContentType, "text/plain"));
            Content = Encoding.UTF8.GetBytes(content);
        }
    }
}
