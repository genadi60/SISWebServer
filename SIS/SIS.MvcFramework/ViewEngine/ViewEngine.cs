using System;
using SIS.MvcFramework.ViewEngine.Contracts;

namespace SIS.MvcFramework.ViewEngine
{
    public class ViewEngine : IViewEngine
    {
        public string GetHtml<T>(string viewName, string viewCode, T model)
        {
            var csharpMethodBody = GenerateCSharpMethodBody(viewCode);
            //1.viewCode => C# code
            string viewCSharpCode = @"
using System;
using System.Linq;
using System.Text;
using System.Collection.Generic;
namespace MyAppView
{
    public class" + viewName + @"View : IView<" + typeof(T).FullName + @">
    {
        public string GetHtml(" + typeof(T).FullName +@" model)
        {
            var html = new StringBuilder();"
            
            + csharpMethodBody + @"

            
            return html.ToString();
        }
    }
}
";
            var instanceOfViewClass = GetInstance(viewCSharpCode) as IView<T>;

            var html = instanceOfViewClass.GetHtml(model);

            //2.C# => executable object.GetHtml(model)
            
            return html;
        }

        private object GetInstance(string viewCSharpCode)
        {
            object view = null;
            return view;
        }

        private string GenerateCSharpMethodBody(string viewCode)
        {
            return "";
        }

    }
}
