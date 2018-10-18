using SIS.HTTP.Enums;

namespace SIS.WebServer.Results
{
    using HTTP.Common;
    using HTTP.Headers;
    using HTTP.Responses;

    public class FileResult : HttpResponse
    {
        public FileResult(byte[] content, HttpResponseStatusCode statusCode) :base(statusCode)
        {
            Headers.Add(new HttpHeader(GlobalConstants.ContentType, "image/png"));
            Content = content;
        }
    }
}
