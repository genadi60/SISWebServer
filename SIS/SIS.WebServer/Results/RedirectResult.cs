namespace SIS.WebServer.Results
{
    using HTTP.Common;
    using HTTP.Enums;
    using HTTP.Headers;
    using HTTP.Responses;

    public class RedirectResult : HttpResponse
    {
        public RedirectResult(string location) 
            : base(HttpResponseStatusCode.See_Other)
        {
            Headers.Add(new HttpHeader(GlobalConstants.Location, location));
        }
    }
}
