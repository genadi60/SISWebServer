namespace SIS.MvcFramework.Services.Contracts
{
    using System;

    public interface IServiceCollection
    {
        void AddService<TSource, TDestination>();

        void AddService<T>(Func<T> p);

        T CreateInstance<T>();

        object CreateInstance(Type type);
    }
}
