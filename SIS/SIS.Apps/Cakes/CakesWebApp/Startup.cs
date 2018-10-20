namespace CakesWebApp
{
    using Controllers;
    using SIS.HTTP.Enums;
    using SIS.MvcFramework.Contracts;
    using SIS.MvcFramework.Logger;
    using SIS.MvcFramework.Logger.Contracts;
    using SIS.WebServer.Routing;
    using SIS.MvcFramework.Services;
    using SIS.MvcFramework.Services.Contracts;
    
    public class Startup : IMvcApplication
    {
        public void Configure(ServerRoutingTable routing)
        {
            routing.Routes[HttpRequestMethod.GET]["/"] = request => new HomeController{Request = request}.Index();
            routing.Routes[HttpRequestMethod.GET]["/about"] = request => new HomeController{Request = request}.About();
            routing.Routes[HttpRequestMethod.GET]["/register"] =
                request => new AccountController{Request = request}.Register();
            routing.Routes[HttpRequestMethod.GET]["/login"] =
                request => new AccountController{Request = request}.Login();
            routing.Routes[HttpRequestMethod.POST]["/register"] =
                request => new AccountController{Request = request}.DoRegister();
            routing.Routes[HttpRequestMethod.POST]["/login"] =
                request => new AccountController{Request = request}.DoLogin();
            routing.Routes[HttpRequestMethod.GET]["/hello"] = request => new HomeController{Request = request}.Hello();
            routing.Routes[HttpRequestMethod.GET]["/logout"] =
                request => new AccountController{Request = request}.Logout();
            routing.Routes[HttpRequestMethod.GET]["/profile"] =
                request => new AccountController{Request = request}.GetProfile();
            routing.Routes[HttpRequestMethod.GET]["/add"] = request => new CakeController{Request = request}.AddCake();
            routing.Routes[HttpRequestMethod.POST]["/add"] =
                request => new CakeController{Request = request}.DoAddCake();
            routing.Routes[HttpRequestMethod.GET]["/search"] = request => new CakeController{Request = request}.Search();
            routing.Routes[HttpRequestMethod.GET]["/details"] =
                request => new CakeController{Request = request}.CakeDetails();
            routing.Routes[HttpRequestMethod.GET]["/cakes"] = request => new CakeController{Request = request}.GetCakes();
            routing.Routes[HttpRequestMethod.GET]["/shopping/add"] = request => new ShoppingController{Request = request}.AddToCart();
            routing.Routes[HttpRequestMethod.GET]["/shopping/cart"] = request => new ShoppingController{Request = request}.ShowCart();
            routing.Routes[HttpRequestMethod.POST]["/shopping/order"] = request => new ShoppingController{Request = request}.FinishOrder();
            routing.Routes[HttpRequestMethod.GET]["/shopping/my-orders"] = request => new ShoppingController{Request = request}.MyOrders();
            routing.Routes[HttpRequestMethod.GET]["/list"] = request => new ShoppingController{Request = request}.Details();
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddService<IHashService, HashService>();
            collection.AddService<IUserCookieService, UserCookieService>();
            collection.AddService<ILogger, FileLogger>();
        }
    }
}
