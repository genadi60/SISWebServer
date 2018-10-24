namespace SIS.MvcFramework.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    using Contracts;
    using Logger.Contracts;


    public class HashService : IHashService
    {
        private readonly ILogger _logger;

        public HashService(ILogger logger)
        {
            _logger = logger;
        }

        public string Hash(string stringToHash)
        {
            using(var sha256 = SHA256.Create())
            {
                stringToHash = stringToHash + "myAppSalt9876789#";  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
                
                // Get the hashed string.  
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();  
                _logger.Log(hash);  
                return hash;  
            }  
        }
    }
}
