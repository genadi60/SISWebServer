using System;
using SIS.HTTP.Enums;

namespace SIS.MvcFramework.Attributes
{
    public class HttpPostAttribute : HttpAttribute
    {
        public HttpPostAttribute(string path = null) : base(path)
        {
        }

        public override HttpRequestMethod Method => HttpRequestMethod.POST;
    }
}
