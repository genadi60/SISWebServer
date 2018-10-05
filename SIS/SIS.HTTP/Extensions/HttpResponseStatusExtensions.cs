namespace SIS.HTTP.Extensions
{
    using Enums;

    public class HttpResponseStatusExtensions
    {
        public string GetResponseLine(HttpResponseStatusCode statusCode)
        {
            return $"{(int)statusCode} {statusCode.ToString().Replace('_', ' ')}";
        }
    }
}
