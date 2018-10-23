using System.IO;
namespace SIS.MvcFramework.Logger
{
    using System;

    using Contracts;
    using HTTP.Common;

    public class FileLogger : ILogger
    {
        private readonly string _filename;

        private static object _lockObject = new object();

        public FileLogger()
        {
            _filename = "logfile.txt";
        }

        public void Log(string message)
        {
            lock (_lockObject)
            {
                File.AppendAllText(_filename, $"{DateTime.UtcNow}: {message}{GlobalConstants.HttpNewLine}");
            }
        }
    }
}
