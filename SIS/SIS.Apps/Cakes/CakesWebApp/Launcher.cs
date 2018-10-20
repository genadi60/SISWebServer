namespace CakesWebApp
{
    using System;
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
            
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/"] = request => new HomeController{Request = request}.Index();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/about"] = request => new HomeController{Request = request}.About();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/register"] =
                request => new AccountController{Request = request}.Register();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/login"] =
                request => new AccountController{Request = request}.Login();
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/register"] =
                request => new AccountController{Request = request}.DoRegister();
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/login"] =
                request => new AccountController{Request = request}.DoLogin();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/hello"] = request => new HomeController{Request = request}.Hello();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/logout"] =
                request => new AccountController{Request = request}.Logout();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/profile"] =
                request => new AccountController{Request = request}.GetProfile();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/add"] = request => new CakeController{Request = request}.AddCake();
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/add"] =
                request => new CakeController{Request = request}.DoAddCake();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/search"] = request => new CakeController{Request = request}.Search();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/details"] =
                request => new CakeController{Request = request}.CakeDetails();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/cakes"] = request => new CakeController{Request = request}.GetCakes();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/shopping/add"] = request => new ShoppingController{Request = request}.AddToCart();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/shopping/cart"] = request => new ShoppingController{Request = request}.ShowCart();
            serverRoutingTable.Routes[HttpRequestMethod.POST]["/shopping/order"] = request => new ShoppingController{Request = request}.FinishOrder();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/shopping/my-orders"] = request => new ShoppingController{Request = request}.MyOrders();
            serverRoutingTable.Routes[HttpRequestMethod.GET]["/list"] = request => new ShoppingController{Request = request}.Details();


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
