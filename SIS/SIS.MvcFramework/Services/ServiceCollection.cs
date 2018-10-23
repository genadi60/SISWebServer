using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace SIS.MvcFramework.Services
{
    using System;
    using System.Collections.Generic;
    using Contracts;

    public class ServiceCollection : IServiceCollection
    {
        private  IDictionary<Type, Type> serviceContainer;

        public ServiceCollection()
        {
            serviceContainer = new Dictionary<Type, Type>();
        }
        
        public void AddService<TSource, TDestination>()
        {
            serviceContainer[typeof(TSource)] = typeof(TDestination);
        }

        public T CreateInstance<T>()
        {
            return (T) CreateInstance(typeof(T));
        }

        public object CreateInstance(Type type)
        {
            if (serviceContainer.ContainsKey(type))
            {
                type = serviceContainer[type];
            }

            if (type.IsAbstract || type.IsInterface)
            {
                throw new Exception($"Type: {type.FullName} cannot be instantiated.");
            }

            var constructor = type.GetConstructors().First();
            var parameters = constructor.GetParameters();

            List<object> parameterObjects = new List<object>();

            foreach (var parameterInfo in parameters)
            {
                var parameterObject = CreateInstance(parameterInfo.ParameterType);
                parameterObjects.Add(parameterObject);
            }

            var obj = constructor.Invoke(parameterObjects.ToArray());

            return obj;
        }
    }
}
