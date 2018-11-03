using System.IO;
using SIS.MvcFramework.Logger;
using SIS.MvcFramework.Logger.Contracts;

namespace SIS.MvcFramework
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    
    using Attributes;
    using Contracts;
    using Extensions;
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
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            IServiceCollection collection = new ServiceCollection();
            collection.AddService<IHashService, HashService>();
            collection.AddService<IUserCookieService, UserCookieService>();
            collection.AddService<ILogger>(() => new FileLogger($"log_{DateTime.UtcNow:dd-MM-yyyy}.txt"));
            application.ConfigureServices(collection);
            
            var serverRoutingTable = new ServerRoutingTable();

            AutoRegisterRoutes(application, serverRoutingTable, collection);

            application.Configure();

            new Server(80, serverRoutingTable).Run();
        }

        private static void AutoRegisterRoutes(IMvcApplication application, ServerRoutingTable routingTable, IServiceCollection serviceCollection)
        {
            var controllers = application.GetType().Assembly.GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Controller)));
                
            foreach (var controller in controllers)
            {
                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                  
                foreach (var methodInfo in methods)
                {
                    var httpAttribute =
                        (HttpAttribute)methodInfo.GetCustomAttributes(true).FirstOrDefault(ca => ca.GetType().IsSubclassOf(typeof(HttpAttribute)));
                    
                    string path = null;
                    var method = HttpRequestMethod.GET;

                    if (httpAttribute != null)
                    {
                        path = httpAttribute.Path;
                        method = httpAttribute.Method;
                    }
                    
                    if (path == null)
                    {
                        var controllerName = controller.Name;
                        if (controllerName.EndsWith("Controller"))
                        {
                            controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
                        }
                        var actionName = methodInfo.Name;
                        path = $"/{controllerName}/{actionName}";
                        
                    }
                    else if (!path.StartsWith("/"))
                    {
                        path = "/" + path;
                    }

                    routingTable.Add(method, path, (request => ExecuteAction(controller, methodInfo, request, serviceCollection)));
                    Console.WriteLine($"Registered route: {controller.Name}.{methodInfo.Name} => {method} => {path}");
                }
            }

            if (!routingTable.Routes[HttpRequestMethod.GET].ContainsKey("/") 
            
                && routingTable.Routes[HttpRequestMethod.GET].ContainsKey("/Home/Index"))
            {
                routingTable.Routes[HttpRequestMethod.GET]["/"] = request =>
                    routingTable.Routes[HttpRequestMethod.GET]["/Home/Index"](request);

                Console.WriteLine($"Registered route: reuse /Home/Index => {HttpRequestMethod.GET} => /");
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
            controllerInstance.ViewEngine = new ViewEngine.ViewEngine();
            
            var parameters = GetMethodParameters(methodInfo, request, serviceCollection).ToArray();

            var httpResponse = methodInfo.Invoke(controllerInstance, parameters) as IHttpResponse;


            return httpResponse;
        }

        private static List<object> GetMethodParameters(MethodBase methodInfo, IHttpRequest request, IServiceCollection serviceCollection )
        {
            var methodParameters = methodInfo.GetParameters();
            var methodParametersObjects = new List<object>();

            foreach (var methodParameter in methodParameters)
            {
                var parameterType = methodParameter.ParameterType;
                var parameterProperties = parameterType.GetProperties();

                if (parameterType.IsValueType || Type.GetTypeCode(parameterType) == TypeCode.String)
                {
                    var stringValue = GetRequestData(request, methodParameter.Name);
                    methodParametersObjects.Add(TryParseProperties(parameterType, stringValue));
                }
                else
                {
                    var methodParameterObject = serviceCollection.CreateInstance(parameterType);
                    
                    foreach (var propertyInfo in parameterProperties)
                    {
                        string stringValue = GetRequestData(request, propertyInfo.Name);

                        var value = TryParseProperties(propertyInfo.PropertyType, stringValue);

                        if (value != null)
                        {
                            propertyInfo.SetMethod.Invoke(methodParameterObject, new[] {value});
                        }
                    }

                    methodParametersObjects.Add(methodParameterObject);
                }
            }

            return methodParametersObjects;
        }

        private static string GetRequestData(IHttpRequest request, string key)
        {
            key = key.ToLower();
            string stringValue = null;

            if (request.FormData.Any(k => k.Key.ToLower() == key))
            {
                stringValue = request.FormData.First(k => k.Key.ToLower() == key).Value.ToString().Trim().UrlDecode();

            }

            else if (request.QueryData.Any(k => k.Key.ToLower() == key))
            {
                stringValue = request.QueryData.First(k => k.Key.ToLower() == key).Value.ToString().Trim().UrlDecode();
            }

            return stringValue;
        }

        private static object TryParseProperties(Type propertyType, string stringValue)
        {
            var typeCode = Type.GetTypeCode(propertyType);
            object value = null;
            switch (typeCode)
            {
                case TypeCode.Char:
                    if (char.TryParse(stringValue, out var charValue)) value = charValue;
                    break;
                case TypeCode.Int32:
                    if (int.TryParse(stringValue, out var intValue)) value = intValue;
                    break;
                case TypeCode.Int64:
                    if (long.TryParse(stringValue, out var longValue)) value = longValue;
                    break;
                case TypeCode.Double:
                    if (double.TryParse(stringValue, out var doubleValue)) value = doubleValue;
                    break;
                case TypeCode.Decimal:
                    if (decimal.TryParse(stringValue, out var decimalValue)) value = decimalValue;
                    break;
                case TypeCode.DateTime:
                    if (DateTime.TryParse(stringValue, out var dateTimeValue)) value = dateTimeValue;
                    break;
                case TypeCode.String:
                    value = stringValue;
                    break;
            }

            return value;
        }
    }
}
