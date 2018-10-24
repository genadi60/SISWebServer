using SIS.MvcFramework.ViewEngine.Contracts;

namespace SIS.MvcFramework.ViewEngine
{
    public class ViewEngine : IViewEngine
    {
        public string GetHtml(string viewCode)
        {
            return viewCode;
        }
    }
}
