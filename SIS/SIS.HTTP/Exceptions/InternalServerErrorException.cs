namespace SIS.HTTP.Exceptions
{
    using System;

    public class InternalServerErrorException : Exception
    {
        public const string ServerExceptionMessage = "The Server has encountered an error.";

        public InternalServerErrorException() : base(ServerExceptionMessage)
        {
        }
    }
}
