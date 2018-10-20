namespace SIS.MvcFramework
{
    using System;

    public class HttpPostAttribute : Attribute
    {
        public string Path { get; }

        public HttpPostAttribute(string path)
        {
            Path = path;
        }
    }
}
