﻿namespace SIS.WebServer
{
    using System;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using HTTP.Common;
    using HTTP.Cookies;
    using HTTP.Enums;
    using HTTP.Requests;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using HTTP.Sessions;
    using Results;
    using Routing;
    
    public class ConnectionHandler
    {
        private readonly Socket _client;

        private readonly ServerRoutingTable _serverRoutingTable;

        public ConnectionHandler(Socket client, ServerRoutingTable serverRoutingTable)
        {
            _client = client;
            _serverRoutingTable = serverRoutingTable;
        }

        public async Task ProcessRequestAsync()
        {
            var httpRequest = await ReadRequest();

            if (httpRequest != null)
            {
                string sessionId = SetRequestSession(httpRequest);

                var httpResponse = HandleRequest(httpRequest);

                SetResponseSession(httpRequest, httpResponse, sessionId);

                await PrepareResponse(httpResponse);
            }

            _client.Shutdown(SocketShutdown.Both);
        }

        private async Task<IHttpRequest> ReadRequest()
        {
            var sb = new StringBuilder();
            var buffer = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                var readLength = await _client.ReceiveAsync(buffer, SocketFlags.None);

                if (readLength == 0)
                {
                    break;
                }

                var requestText = Encoding.UTF8.GetString(buffer.Array, 0, readLength);
                sb.Append(requestText);

                if (readLength < 1023)
                {
                    break;
                }
            }
            var requestString = sb.ToString();

            if (requestString.Length == 0)
            {
                return null;
            }

            Console.WriteLine("-----REQUEST-----");
            Console.WriteLine(requestString);
            Console.WriteLine();

            return new HttpRequest(requestString);
        }

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            
            var parsedPath = ParseRoute(httpRequest);

            if (!_serverRoutingTable.Routes.ContainsKey(httpRequest.RequestMethod)
                || !_serverRoutingTable.Routes[httpRequest.RequestMethod].ContainsKey(parsedPath))
            {
                return NotFound.PageNotFound();
            }
            var response = _serverRoutingTable.Routes[httpRequest.RequestMethod][parsedPath].Invoke(httpRequest);

            return response;
        }

        private string ParseRoute(IHttpRequest httpRequest)
        {
            var httpRequestPath = httpRequest.Path;
            var parsedRegex = new StringBuilder();

            ParsePath(httpRequest.RequestMethod, httpRequestPath, parsedRegex);

            return parsedRegex.ToString().Trim();
        }

        private void ParsePath(HttpRequestMethod requestMethod, string httpRequestPath, StringBuilder parsedRegex)
        {
            var availableRoutes = _serverRoutingTable.Routes
                    .Where(k => k.Key.Equals(requestMethod))
                    .Select(k => k.Value)
                    .Select(v => v.Keys)
                    .ToList();

            foreach (var routes in availableRoutes)
            {
                foreach (var route in routes)
                {
                    string pattern = $"^{route}$";
                    var regex = new Regex(pattern);
                    var match = regex.Match(httpRequestPath);

                    if (!match.Success)
                    {
                        continue;
                    }

                    parsedRegex.Append(route);
                }
            }
        }

        private async Task PrepareResponse(IHttpResponse httpResponse)
        {
            if (httpResponse != null)
            {
                var responseBytes = httpResponse.GetBytes();
                var byteSegments = new ArraySegment<byte>(responseBytes);

                await _client.SendAsync(byteSegments, SocketFlags.None);

                Console.WriteLine("----RESPONSE----");
                Console.WriteLine(Encoding.UTF8.GetString(responseBytes));
                Console.WriteLine();
            }
            _client.Shutdown(SocketShutdown.Both);
        }

        private string SetRequestSession(IHttpRequest httpRequest)
        {
            string sessionId;

            if (httpRequest.Cookies.ContainsCookie(GlobalConstants.SessionCookieKey))
            {
                var cookie = httpRequest.Cookies.GetCookie(GlobalConstants.SessionCookieKey);
                sessionId = cookie.Value;
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
                httpRequest.Session = HttpSessionStorage.GetSession(sessionId);
            }

            return sessionId;
        }

        private void SetResponseSession(IHttpRequest httpRequest, IHttpResponse httpResponse, string sessionId)
        {
            if (sessionId != null)
            {
                if (!httpResponse.Cookies.ContainsCookie(GlobalConstants.SessionCookieKey)
                    && !httpRequest.Cookies.ContainsCookie(GlobalConstants.SessionCookieKey))
                {
                    httpResponse.Cookies
                        .Add(new HttpCookie(GlobalConstants.SessionCookieKey,
                            $"{sessionId}; {GlobalConstants.HttpOnly}"));
                }
            }
        }
    }
}
