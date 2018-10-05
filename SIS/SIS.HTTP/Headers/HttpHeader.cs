namespace SIS.HTTP.Headers
{
    using Contracts;

    public class HttpHeader : IHttpHeader
    {
        public HttpHeader(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"{Key}: {Value}";
        }
    }
}
