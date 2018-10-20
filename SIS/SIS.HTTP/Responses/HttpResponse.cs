namespace SIS.HTTP.Responses
{
    using System.Linq;
    using System.Text;

    using Common;
    using Contracts;
    using Cookies;
    using Cookies.Contracts;
    using Enums;
    using Extensions;
    using Headers;
    using Headers.Contracts;

    public class HttpResponse : IHttpResponse
    {
        private string _statusCodeLine => new HttpResponseStatusExtensions().GetResponseLine(StatusCode);

        public HttpResponse()
        {
            Headers = new HttpHeaderCollection();
            Cookies = new HttpCookieCollection();
            Content = new byte[0];
        }

        public HttpResponse(HttpResponseStatusCode statusCode) : this()
        {
            StatusCode = statusCode;
        }

        public HttpResponseStatusCode StatusCode { get; set; } = HttpResponseStatusCode.OK;

        public IHttpHeaderCollection Headers { get; set; }

        public IHttpCookieCollection Cookies { get; set; }

        public byte[] Content { get; set; }

        public void AddHeader(HttpHeader header)
        {
            Headers.Add(header);
        }

        public void AddCookie(HttpCookie cookie)
        {
            Cookies.Add(cookie);
        }

        public byte[] GetBytes()
        {
            var bytes = Encoding.UTF8.GetBytes($"{ToString()}");

            if (Content.Length > 0)
            {
                bytes = bytes.Concat(Content).ToArray();
            }

            return bytes;
        }

        public override string ToString()
        {
            bool existsContent = Content.Length > 0;

            var sb = new StringBuilder();

            sb.Append(
                    $"{GlobalConstants.HttpOneProtocolFragment} {_statusCodeLine}{GlobalConstants.HttpNewLine}")
                .Append(Headers.ToString());

            if (Cookies.HasCookies())
            {
                foreach (var httpCookie in Cookies)
                {
                    sb.Append($"{GlobalConstants.SetCookie}{httpCookie}{GlobalConstants.HttpNewLine}");
                }
                
            }

            if (existsContent)
            {
                sb.Append($"{GlobalConstants.ContentLength}{Content.Length}{GlobalConstants.HttpNewLine}");
            }
            sb.Append($"{GlobalConstants.HttpNewLine}{GlobalConstants.HttpNewLine}");
            

            var response = sb.ToString();
            return response;
        }
    }
}
