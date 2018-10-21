namespace SIS.MvcFramework.Logger
{
    using System;

    using Contracts;

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
