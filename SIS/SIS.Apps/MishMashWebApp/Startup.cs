using MishMashWebApp.Services;
using MishMashWebApp.Services.Contracts;
using SIS.MvcFramework.Contracts;
using SIS.MvcFramework.Services.Contracts;

namespace MishMashWebApp
{
    public class Startup : IMvcApplication
    {
        public void Configure()
        {
        }
        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IUserService, UserService>();
        }
    }
}
