namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Common;
    using Contracts;
    using Cookies;
    using Cookies.Contracts;
    using Enums;
    using Exceptions;
    using Headers;
    using Headers.Contracts;
    using Sessions.Contracts;
    

    public class HttpRequest : IHttpRequest
    {
        public HttpRequest(string requestString)
        {
            FormData = new Dictionary<string, object>();
            QueryData = new Dictionary<string, object>();
            Headers = new HttpHeaderCollection();
            Cookies = new HttpCookieCollection();

            ParseRequest(requestString);
        }

        public IHttpSession Session { get; set; }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        private bool IsValidRequestLine(string[] requestLine) 
            => requestLine.Length == 3 && GlobalConstants.HttpOneProtocolFragment.Equals(requestLine[2]);
       
        private bool IsValidRequestQueryString(string queryString, string[] queryParameters) 
            => !string.IsNullOrEmpty(queryString) && queryParameters.Length >= 1;
        
        private void ParseRequestMethod(string[] requestLine)
        {
            if(Enum.TryParse(requestLine[0], out HttpRequestMethod requestMethod))
            {
                RequestMethod = requestMethod;
            }
        }

        private void ParseRequestUrl(string[] requestLine)
        {
            Url = requestLine[1];
        }

        private void ParseRequestPath()
        {
            Path = Url
                .Split('?', '#')
                .First();
        }

        private void ParseHeaders(string[] requestContent)
        {
            foreach (var contentLine in requestContent)
            {
                if (contentLine.Equals(string.Empty))
                {
                    break;
                }

                var headerTokens = contentLine
                    .Split(": ")
                    .ToArray();

                if (headerTokens.Length != 2)
                {
                    throw new BadRequestException();
                }

                var key = headerTokens[0].Trim();
                var value = headerTokens[1].Trim();
                var header = new HttpHeader(key, value);
                Headers.Add(header);
            }
            
            if (!Headers.ContainsHeader(GlobalConstants.HostHeaderKey))
            {
                throw new BadRequestException();
            }
        }

        private void ParseQueryParameters()
        {
            if (!Url.Contains('?'))
            {
                return;
            }

            var queryString = Url
                .Split(new[] {'?'}, StringSplitOptions.RemoveEmptyEntries)
                .Last();
            ParseQuery(queryString, QueryData);
            
        }

        private void ParseFormDataParameters(string formData)
        {
            ParseQuery(formData, FormData);
        }

        private void ParseRequestParameters(string formData)
        {
            ParseQueryParameters();

            if (RequestMethod == HttpRequestMethod.GET)
            {
                return;
            }

            ParseFormDataParameters(formData);
        }

        private void ParseRequest(string requestString)
        {
            var splitRequestContent = requestString
                .Split(new []{Environment.NewLine}, StringSplitOptions.None)
                .ToArray();

            var requestLine = splitRequestContent
                .First()
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            if (!IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }

            ParseRequestMethod(requestLine);
            ParseRequestUrl(requestLine);
            ParseRequestPath();

            var requestHeaders = splitRequestContent
                .Skip(1)
                .ToArray();
            ParseHeaders(requestHeaders);
            ParseCookies();

            var formData = requestHeaders
                .Last();
            ParseRequestParameters(formData);
        }

        private void ParseCookies()
        {
            if (Headers.ContainsHeader(GlobalConstants.HeaderCookieKey))
            {
                var requestCookies = Headers.GetHeader(GlobalConstants.HeaderCookieKey).Value;

                if (!requestCookies.Contains(GlobalConstants.ParameterKvpSeparator))
                {
                    throw new BadRequestException();
                }

                var cookies = requestCookies
                    .Split("; ")
                    .ToArray();

                foreach (var cookie in cookies)
                {
                    var cookieParts = cookie
                        .Split('=', 2)
                        .ToArray();

                    if (cookieParts.Length != 2)
                    {
                        continue;
                    }

                    var key = cookieParts[0];
                    var value = cookieParts[1];

                    var httpCookie = new HttpCookie(key, value, false);

                    Cookies.Add(httpCookie);
                }
            }
        }

        private void ParseQuery(string queryString, Dictionary<string, object> dictionary)
        {
            if (!queryString.Contains(GlobalConstants.ParameterKvpSeparator))
            {
                return;
            }

            var queryParameters = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);

            if (IsValidRequestQueryString(queryString, queryParameters))
            {
                foreach (var queryPair in queryParameters)
                {
                    var queryKvp = queryPair.Split(GlobalConstants.ParameterKvpSeparator, StringSplitOptions.RemoveEmptyEntries);

                    if (queryKvp.Length != 2)
                    {
                        return;
                    }

                    var key = WebUtility.UrlDecode(queryKvp[0]);
                    var value = WebUtility.UrlDecode(queryKvp[1]);

                    dictionary.Add(key, value);
                }
            }
        }
    }
}
