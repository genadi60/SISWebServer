namespace SIS.HTTP.Cookies
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Contracts;

    public class HttpCookieCollection : IHttpCookieCollection
    {
        private readonly Dictionary<string, HttpCookie> _cookies;

        public HttpCookieCollection()
        {
            _cookies = new Dictionary<string, HttpCookie>();
        }

        public void Add(HttpCookie cookie)
        {
            if (cookie == null)
            {
                throw new ArgumentNullException();
            }
            _cookies.Add(cookie.Key, cookie);
        }

        public void Add(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                var cookie = new HttpCookie(key, value);
                Add(cookie);
            }
        }

        public bool ContainsCookie(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException();
            }

            return _cookies.ContainsKey(key);
        }

        public HttpCookie GetCookie(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException();
            }

            return ContainsCookie(key) ? _cookies[key] : throw new InvalidOperationException($"The given key: {key} is not present in the cookies collection.");
        }

        public bool HasCookies()
        {
            return _cookies.Count > 0;
        }

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            foreach (var cookie in _cookies)
            {
                yield return cookie.Value;
            }
        }

        public override string ToString()
        {
            return string.Join("; ", _cookies.Values);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
