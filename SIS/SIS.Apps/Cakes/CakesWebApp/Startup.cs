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
            //collection.AddService<ILogger, ConsoleLogger>();
            collection.AddService<ILogger, FileLogger>();
        }
    }
}
