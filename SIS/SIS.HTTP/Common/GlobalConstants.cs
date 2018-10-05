namespace SIS.HTTP.Common
{
    public static class GlobalConstants
    {
        public const string HttpOneProtocolFragment = "HTTP/1.1";

        public const string HostHeaderKey = "Host";

        public const string HeaderCookieKey = "Cookie";

        public const string ParameterKvpSeparator = "=";

        public const string SessionId = "SessionId";

        public const string SetCookie = "Set-Cookie: ";

        public const string ContentLength = "Content-Length: ";

        public const string ContentType = "Content-Type";

        public const string HttpOnly = "HttpOnly";

        public const string StartMessage = "Server started at http://{0}:{1}";

        public const string LocalhostIpAddress = "::1";

        public const string NotFoundPage = "<h1>This Page Not Found!</h1><div>{0}</div>";

        public const string Location = "Location";

        public const string SessionCookieKey = "SIS_ID";

        public const string GreetingString = "<h1>Hello, World!</h1><div>{0}</div>";
    }
}
