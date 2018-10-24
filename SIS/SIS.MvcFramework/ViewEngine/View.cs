using SIS.MvcFramework.ViewEngine.Contracts;

namespace SIS.MvcFramework.ViewEngine
{
    public class View<T> : IView<T>
    {
        public string GetHtml(T model)
        {
            return "";
        }
    }
}
