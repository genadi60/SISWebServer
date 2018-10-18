namespace SIS.MvcFramework.Services
{
    using System;

    using Contracts;

    public class ServiceCollection : IServiceCollection
    {
        public void AddService<TSource, TDestination>()
        {
            throw new NotImplementedException();
        }

        public T CreateInstance<T>()
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
