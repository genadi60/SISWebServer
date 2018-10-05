namespace SIS.HTTP.Sessions
{
    using System;
    using System.Collections.Generic;

    using Contracts;

    public class HttpSession : IHttpSession
    {
        private readonly Dictionary<string, object> _parameters;

        public HttpSession(string id)
        {
            _parameters = new Dictionary<string, object>();
            Id = id;
        }

        public string Id { get; }
        public object GetParameter(string name)
        {
            if (string.IsNullOrEmpty(name) || !_parameters.ContainsKey(name))
            {
                throw new ArgumentException();
            }

            return _parameters[name];
        }

        public bool ContainsParameter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException();
            }

            return _parameters.ContainsKey(name);
        }

        public void AddParameter(string name, object parameter)
        {
            if (string.IsNullOrEmpty(name) || parameter == null)
            {
                throw new ArgumentNullException();
            }

            _parameters.TryAdd(name, parameter);
        }

        public void ClearParameters()
        {
            _parameters.Clear();
        }
    }
}
