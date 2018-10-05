namespace SIS.HTTP.Cookies
{
    using System;

    using Contracts;

    public class HttpCookie : IHttpCookie
    {
        private const int HttpCookieDefaultExpirationDays = 3;

        public HttpCookie(string key, string value, int expires = HttpCookieDefaultExpirationDays)
        {
            Key = key;
            Value = value;
            IsNew = true;
            Expires = DateTime.UtcNow.AddDays(expires);
        }

        public HttpCookie(string key, string value, bool isNew, int expires = HttpCookieDefaultExpirationDays) 
            : this(key, value, expires)
        {
            IsNew = isNew;
        }

        public string Key { get; set; }

        public string Value { get; set; }

        public DateTime Expires { get; private set; }

        public bool IsNew { get; set; }

        public void Delete()
        {
            Expires = DateTime.UtcNow.AddDays(-1);
        }

        public override string ToString() => $"{Key}={Value}; Expires={Expires:R}";
        
    }
}
