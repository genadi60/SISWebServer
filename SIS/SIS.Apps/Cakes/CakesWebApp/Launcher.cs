namespace CakesWebApp
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.EntityFrameworkCore;

    using Controllers;
    using Data;
    using SIS.HTTP.Enums;
    using SIS.WebServer;
    using SIS.WebServer.Routing;

    public class Launcher
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            InitializeDatabase();

            var serverRoutingTable = new ServerRoutingTable();
            Dictionary<string, string> viewData = new Dictionary<string,string>();

            
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/"] = request => new HomeController(viewData).Index(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/favicon"] = request => new HomeController(viewData).Favicon(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/about"] = request => new HomeController(viewData).About(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/register"] =
                request => new AccountController(viewData).Register(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/login"] =
                request => new AccountController(viewData).Login(request);
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/register"] =
                request => new AccountController(viewData).DoRegister(request);
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/login"] =
                request => new AccountController(viewData).DoLogin(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/hello"] = request => new HomeController(viewData).Hello(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/logout"] =
                request => new AccountController(viewData).Logout(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/profile"] =
                request => new AccountController(viewData).GetProfile(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/add"] = request => new CakeController(viewData).AddCake(request);
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/add"] =
                request => new CakeController(viewData).DoAddCake(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/search"] = request => new CakeController(viewData).Search(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/details/(?<id>[0-9]+)"] =
                request => new CakeController(viewData).CakeDetails(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/cakes"] = request => new CakeController(viewData).GetCakes(request);
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/shopping/add"] = request => new ShoppingController(viewData).AddToCart(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/shopping/cart"] = request => new ShoppingController(viewData).ShowCart(request);
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/shopping/order"] = request => new ShoppingController(viewData).FinishOrder(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/shopping/my-orders"] = request => new ShoppingController(viewData).MyOrders(request);
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/list"] = request => new ShoppingController(viewData).Details(request);


            new Server(80, serverRoutingTable).Run();
        }

        private static void InitializeDatabase()
        {
            using (var db = new CakesDbContext())
            {
                db.Database.Migrate();
            }
        }
    }
}
