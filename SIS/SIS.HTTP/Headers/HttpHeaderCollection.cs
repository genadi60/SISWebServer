namespace SIS.HTTP.Headers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Contracts;

    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private readonly Dictionary<string, HttpHeader> _headers;

        public HttpHeaderCollection()
        {
            _headers = new Dictionary<string, HttpHeader>();
        }

        public void Add(HttpHeader header)
        {
            _headers.Add(header.Key, header);
        }

        public bool ContainsHeader(string key)
        {
            return _headers.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            if (!ContainsHeader(key))
            {
                return null;
            }

            return _headers[key];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var header in _headers.Values)
            {
                sb.Append($"{header}{Environment.NewLine}");
            }

            return sb.ToString();
        }
    }
}
