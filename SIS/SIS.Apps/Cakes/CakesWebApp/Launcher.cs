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

            serverRoutingTable.Routes[HttpRequestMethod.GET]["/"] = request => new HomeController(request, viewData).Index();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/about"] = request => new HomeController(request, viewData).About();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/register"] =
                request => new AccountController(request, viewData).Register();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/login"] =
                request => new AccountController(request, viewData).Login();
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/register"] =
                request => new AccountController(request, viewData).DoRegister();
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/login"] =
                request => new AccountController(request, viewData).DoLogin();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/hello"] = request => new HomeController(request, viewData).Hello();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/logout"] =
                request => new AccountController(request, viewData).Logout();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/profile"] =
                request => new AccountController(request, viewData).GetProfile();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/add"] = request => new CakeController(request, viewData).AddCake();
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/add"] =
                request => new CakeController(request, viewData).DoAddCake();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/search"] = request => new CakeController(request, viewData).Search();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/details"] =
                request => new CakeController(request, viewData).CakeDetails();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/cakes"] = request => new CakeController(request, viewData).GetCakes();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/shopping/add"] = request => new ShoppingController(request, viewData).AddToCart();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/shopping/cart"] = request => new ShoppingController(request, viewData).ShowCart();
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/shopping/order"] = request => new ShoppingController(request, viewData).FinishOrder();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/shopping/my-orders"] = request => new ShoppingController(request, viewData).MyOrders();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/list"] = request => new ShoppingController(request, viewData).Details();


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
