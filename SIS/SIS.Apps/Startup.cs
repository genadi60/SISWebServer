namespace CakesWebApp
{
    using Services;
    using Services.Contracts;
    using SIS.MvcFramework.Contracts;
    using SIS.MvcFramework.Services.Contracts;
    
    public class Startup : IMvcApplication
    {
        public void Configure()
        {
        }
        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IUserService, UserService>();
            collection.AddService<IAccountService, AccountService>();
        }
    }
}
