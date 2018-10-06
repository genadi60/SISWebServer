namespace CakesWebApp.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class HashService : IHashService
    {
        public string Hash(string stringToHash)
        {
            using(var sha256 = SHA256.Create())
            {
                stringToHash = stringToHash + "myAppSalt9876789#";  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));  
                // Get the hashed string.  
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();  
                  
                return hash;  
            }  
        }
    }
}
