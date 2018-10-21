namespace SIS.MvcFramework
{
    using System;
    using System.Linq;
    using System.Reflection;
    
    using Attributes;
    using Contracts;
    using HTTP.Enums;
    using HTTP.Requests.Contracts;
    using HTTP.Responses.Contracts;
    using Services;
    using Services.Contracts;
    using WebServer;
    using WebServer.Results;
    using WebServer.Routing;

    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            application.ConfigureServices(serviceCollection);

            var serverRoutingTable = new ServerRoutingTable();

            AutoRegisterRoutes(serverRoutingTable, application, serviceCollection);

            application.Configure();

            new Server(80, serverRoutingTable).Run();
        }

        private static void AutoRegisterRoutes(ServerRoutingTable routingTable, IMvcApplication application, IServiceCollection serviceCollection)
        {
            var controllers = application.GetType().Assembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Controller)));
                
            foreach (var controller in controllers)
            {
                var methods = controller.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.CustomAttributes.Any(ca => ca.AttributeType.IsSubclassOf(typeof(HttpAttribute))));
                   
                foreach (var method in methods)
                {
                    var httpAttribute =
                        (HttpAttribute)method.GetCustomAttributes(true).FirstOrDefault(ca => ca.GetType().IsSubclassOf(typeof(HttpAttribute)));

                    if (httpAttribute == null)
                    {
                        continue;
                    }

                    routingTable.Add(httpAttribute.Method, httpAttribute.Path, (request => ExecuteAction(controller, method, request, serviceCollection)));
                    
                    Console.WriteLine($"{controller.FullName} => {method.Name} => {httpAttribute.Method} => {httpAttribute.Path}");
                }

                Console.WriteLine();
                
            }
            
        }

        private static IHttpResponse ExecuteAction(Type controllerType, MethodBase methodInfo, IHttpRequest request, IServiceCollection serviceCollection )
        {
            var controllerInstance = serviceCollection.CreateInstance(controllerType) as Controller;
            if (controllerInstance == null)
            {
                return new TextResult("Controller not found.", HttpResponseStatusCode.Internal_Server_Error);
            }

            controllerInstance.Request = request;
            controllerInstance.UserCookieService = serviceCollection.CreateInstance<IUserCookieService>();
            controllerInstance.HashService = serviceCollection.CreateInstance<IHashService>();

            var httpResponse = methodInfo.Invoke(controllerInstance, new object[] { }) as IHttpResponse;

            
            return httpResponse;
        }
    }
}
