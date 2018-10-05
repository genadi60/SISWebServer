namespace SIS.HTTP.Cookies.Contracts
{
    using System.Collections.Generic;

    public interface IHttpCookieCollection : IEnumerable<HttpCookie>
    {
        void Add(HttpCookie cookie);

        void Add(string key, string value);

        bool ContainsCookie(string key);

        HttpCookie GetCookie(string key);

        bool HasCookies();

        string ToString();
    }
}
