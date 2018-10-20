namespace SIS.MvcFramework
{
    using Contracts;
    using Services;
    using Services.Contracts;
    using WebServer;
    using WebServer.Routing;
    
    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            ////IServiceCollection collection = new ServiceCollection();
            ////application.ConfigureServices(collection);

            var serverRoutingTable = new ServerRoutingTable();
            application.Configure(serverRoutingTable);

            new Server(80, serverRoutingTable).Run();
        }
    }
}
