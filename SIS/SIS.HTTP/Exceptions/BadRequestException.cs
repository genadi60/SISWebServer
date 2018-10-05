namespace SIS.HTTP.Exceptions
{
    using System;

    public class BadRequestException : Exception
    {
        private const string MalformedRequestMessage = "The Request was malformed or contains unsupported elements.";

        public static object ThrowFromInvalidRequest()
       
            => throw new BadRequestException();
       
        public BadRequestException() 
            : base(MalformedRequestMessage)
        {
        }
    }
}
