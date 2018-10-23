namespace SIS.MvcFramework.Contracts
{
    using Services.Contracts;
    using WebServer.Routing;


    public interface IMvcApplication
    {
        void Configure();

        void ConfigureServices(IServiceCollection collection);
    }
}
