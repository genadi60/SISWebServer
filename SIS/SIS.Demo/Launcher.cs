namespace SIS.Demo
{
    using System;
    using System.Text;

    using Controllers;
    using HTTP.Enums;
    using WebServer;
    using WebServer.Routing;

    public class Launcher
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            
            var routingTable = new ServerRoutingTable();

            routingTable.Routes[HttpRequestMethod.GET]["/"] = request => new HomeController().Index();
            
            new Server(80, routingTable).Run();
        }
    }
}
