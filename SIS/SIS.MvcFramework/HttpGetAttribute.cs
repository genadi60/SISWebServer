namespace SIS.MvcFramework
{
    using System;

    public class HttpGetAttribute : Attribute
    {
        public string Path { get; }

        public HttpGetAttribute(string path)
        {
            Path = path;
        }
    }
}
