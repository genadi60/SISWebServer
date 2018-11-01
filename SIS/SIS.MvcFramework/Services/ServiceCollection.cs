namespace SIS.MvcFramework.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    public class ServiceCollection : IServiceCollection
    {
        private readonly IDictionary<Type, Type> _serviceContainer;

        private readonly IDictionary<Type, Func<object>> _serviceFuncsContainer;

        public ServiceCollection()
        {
            _serviceContainer = new Dictionary<Type, Type>();
            _serviceFuncsContainer = new Dictionary<Type, Func<object>>();
        }
        
        public void AddService<TSource, TDestination>()
        {
            _serviceContainer[typeof(TSource)] = typeof(TDestination);
        }

        public void AddService<T>(Func<T> p)
        {
            _serviceFuncsContainer[typeof(T)] = () => p();
        }

        public T CreateInstance<T>()
        {
            return (T) CreateInstance(typeof(T));
        }

        public object CreateInstance(Type type)
        {
            if (_serviceFuncsContainer.ContainsKey(type))
            {
                return _serviceFuncsContainer[type]();
            }

            if (_serviceContainer.ContainsKey(type))
            {
                type = _serviceContainer[type];
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
