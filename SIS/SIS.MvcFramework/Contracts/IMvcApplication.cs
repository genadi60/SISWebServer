namespace SIS.MvcFramework.Contracts
{
    using Services.Contracts;
    using WebServer.Routing;


    public interface IMvcApplication
    {
        void Configure(ServerRoutingTable routing);

        void ConfigureServices(IServiceCollection collection);
    }
}
