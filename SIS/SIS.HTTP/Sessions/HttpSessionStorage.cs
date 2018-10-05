namespace SIS.HTTP.Sessions
{
    using System.Collections.Concurrent;

    using Contracts;

    public class HttpSessionStorage
    {
        private static readonly ConcurrentDictionary<string, HttpSession> _sessions 
            = new ConcurrentDictionary<string, HttpSession>();

        public static IHttpSession GetSession(string id)
        {
            return _sessions.GetOrAdd(id, _ => new HttpSession(id));
        }
    }
}
