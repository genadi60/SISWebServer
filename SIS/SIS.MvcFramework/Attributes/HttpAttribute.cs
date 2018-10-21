using SIS.HTTP.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SIS.MvcFramework.Attributes
{
    public abstract class HttpAttribute : Attribute
    {
        public string Path { get; }
        public abstract HttpRequestMethod Method { get; }

        protected HttpAttribute(string path)
        {
            Path = path;
        }
    }
}
