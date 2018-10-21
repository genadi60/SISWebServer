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
    using WebServer;
    using WebServer.Results;
    using WebServer.Routing;

    public static class WebHost
    {
        public static void Start(IMvcApplication application)
        {
            ////IServiceCollection collection = new ServiceCollection();
            ////application.ConfigureServices(collection);

            var serverRoutingTable = new ServerRoutingTable();

            AutoRegisterRoutes(serverRoutingTable, application);

            application.Configure();

            new Server(80, serverRoutingTable).Run();
        }

        private static void AutoRegisterRoutes(ServerRoutingTable routingTable, IMvcApplication application)
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

                    routingTable.Add(httpAttribute.Method, httpAttribute.Path, (request => ExecuteAction(controller, method, request)));
                    
                    Console.WriteLine($"{controller.FullName} => {method.Name} => {httpAttribute.Method} => {httpAttribute.Path}");
                }

                Console.WriteLine();
                
            }
            
        }

        private static IHttpResponse ExecuteAction(Type controllerType, MethodBase methodInfo, IHttpRequest request )
        {
            var controllerInstance = Activator.CreateInstance(controllerType) as Controller;
            if (controllerInstance == null)
            {
                return new TextResult("Controller not found.", HttpResponseStatusCode.Internal_Server_Error);
            }

            controllerInstance.Request = request;

            var httpResponse = methodInfo.Invoke(controllerInstance, new object[] { }) as IHttpResponse;

            
            return httpResponse;
        }
    }
}
