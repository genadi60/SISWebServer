using SIS.HTTP.Enums;
namespace SIS.MvcFramework.Attributes
{
    using System;

    public abstract class HttpAttribute : Attribute
    {
        public string Path { get; }
        public abstract HttpRequestMethod Method { get; }

        protected HttpAttribute(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            Path = path;
        }
    }
}
