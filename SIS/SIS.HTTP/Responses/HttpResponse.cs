namespace SIS.HTTP.Responses
{
    using System;
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
        private readonly string _statusCodeLine;

        public HttpResponse() { }

        public HttpResponse(HttpResponseStatusCode statusCode)
        {
            Headers = new HttpHeaderCollection();
            Cookies = new HttpCookieCollection();
            Content = new byte[0];
            StatusCode = statusCode;
            _statusCodeLine = new HttpResponseStatusExtensions().GetResponseLine(StatusCode);
        }

        public HttpResponseStatusCode StatusCode { get; set; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; private set; }

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
                    $"{GlobalConstants.HttpOneProtocolFragment} {_statusCodeLine}{Environment.NewLine}")
                .Append(Headers.ToString());

            if (Cookies.HasCookies())
            {
                foreach (var httpCookie in Cookies)
                {
                    sb.Append($"{GlobalConstants.SetCookie}{httpCookie}{Environment.NewLine}");
                }
                
            }

            if (existsContent)
            {
                sb.Append($"{GlobalConstants.ContentLength}{Content.Length}{Environment.NewLine}");
            }
            sb.Append($"{Environment.NewLine}{Environment.NewLine}");
            

            var response = sb.ToString();
            return response;
        }
    }
}
