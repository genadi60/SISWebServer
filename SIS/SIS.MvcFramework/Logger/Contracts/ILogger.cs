using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.MvcFramework.Logger.Contracts
{
    public interface ILogger
    {
        void Log(string message);
    }
}
