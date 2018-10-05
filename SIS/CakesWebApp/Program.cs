namespace CakesWebApp
{
    using System;
    using System.Text;

    using Controllers;
    using Data;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;
    
    public class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            var context = new CakesDbContext();

            var routingTable = new ServerRoutingTable();

            routingTable.Routes[HttpRequestMethod.GET]["/"] = request => new HomeController().Index(request);
            routingTable.Routes[HttpRequestMethod.GET]["/register"] = request => new AccountController().Register(request);
            routingTable.Routes[HttpRequestMethod.GET]["/login"] = request => new AccountController().Login(request);
            routingTable.Routes[HttpRequestMethod.POST]["/register"] = request => new AccountController().DoRegister(request);
            routingTable.Routes[HttpRequestMethod.POST]["/login"] = request => new AccountController().DoLogin(request);
            routingTable.Routes[HttpRequestMethod.GET]["/hello"] = request => new HomeController().Hello(request);
            routingTable.Routes[HttpRequestMethod.GET]["/logout"] = request => new AccountController().Logout(request);

            new Server(80, routingTable).Run();
        }
    }
}
