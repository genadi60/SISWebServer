namespace SIS.MvcFramework.Logger
{
    using System;
    using System.IO;

    using Contracts;
    using HTTP.Common;

    public class FileLogger : ILogger
    {
        private readonly string _filename;

        private static readonly object LockObject = new object();

        public FileLogger()
            : this($"log_{DateTime.UtcNow:dd-MM-yyyy}.txt")
        {

        }

        public FileLogger(string fileName)
        {
            _filename = fileName;
        }

        public void Log(string message)
        {
            lock (LockObject)
            {
                File.AppendAllText(_filename, $"[{DateTime.UtcNow:hh:mm:ss tt}]: {message}{GlobalConstants.HttpNewLine}");
            }
        }
    }
}
