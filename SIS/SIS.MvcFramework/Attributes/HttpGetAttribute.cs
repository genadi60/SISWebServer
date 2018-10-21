using System;
using System.Net.Http;
using SIS.HTTP.Enums;

namespace SIS.MvcFramework.Attributes
{
    public class HttpGetAttribute : HttpAttribute
    {
        public HttpGetAttribute(string path) : base(path)
        {
        }

        public override HttpRequestMethod Method => HttpRequestMethod.GET;
    }
}
