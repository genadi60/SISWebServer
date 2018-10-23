namespace SIS.HTTP.Sessions
{
    using System.Collections.Concurrent;

    using Contracts;

    public class HttpSessionStorage
    {
        private static readonly ConcurrentDictionary<string, HttpSession> Sessions 
            = new ConcurrentDictionary<string, HttpSession>();

        public static IHttpSession GetSession(string id)
        {
            return Sessions.GetOrAdd(id, _ => new HttpSession(id));
        }
    }
}
