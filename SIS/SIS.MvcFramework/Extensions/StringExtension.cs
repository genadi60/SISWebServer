using System;
namespace SIS.MvcFramework.Extensions
{
    using System.Net;

    public static class StringExtension
    {
        public static string UrlDecode(this string input)
        {
            return WebUtility.UrlDecode(input);
        }
    }
}
