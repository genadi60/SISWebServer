using System;
using CakesWebApp.Services;
using CakesWebApp.Services.Contracts;

namespace CakesWebApp
{
    using SIS.MvcFramework.Contracts;
    using SIS.MvcFramework.Logger;
    using SIS.MvcFramework.Logger.Contracts;
    using SIS.MvcFramework.Services;
    using SIS.MvcFramework.Services.Contracts;
    
    public class Startup : IMvcApplication
    {
        public void Configure()
        {
        }
        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IHashService, HashService>();
            collection.AddService<IUserCookieService, UserCookieService>();
            collection.AddService<IUserService, UserService>();
            collection.AddService<IAccountService, AccountService>();
            collection.AddService<ILogger>(() => new FileLogger($"log_{DateTime.UtcNow:dd-MM-yyyy}.txt"));
        }
    }
}
