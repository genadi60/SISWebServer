using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using SIS.MvcFramework.ViewEngine.Contracts;

namespace SIS.MvcFramework.ViewEngine
{
    public class ViewEngine : IViewEngine
    {
        public string GetHtml<T>(string viewName, string viewCode, T model)
        {
            var viewTypeName = viewName + "View";

            var csharpMethodBody = GenerateCSharpMethodBody(viewCode);
            //1.viewCode => C# code
            string viewCSharpCode = @"
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using SIS.MvcFramework.ViewEngine;
using SIS.MvcFramework.ViewEngine.Contracts;
using " + typeof(T).Namespace + @";
namespace MyAppView
{
    public class " + viewTypeName + @" : IView<" + typeof(T).FullName.Replace("+", ".") + @">
    {
        public string GetHtml(" + typeof(T).FullName.Replace("+", ".") + @" model)
        {
            var html = new StringBuilder();"

            + csharpMethodBody + @"

            
            return html.ToString();
        }
    }
}
";
            var instanceOfViewClass = GetInstance(viewCSharpCode, "MyAppView." + viewTypeName, typeof(T)) as IView<T>;

            var html = instanceOfViewClass.GetHtml(model);

            //2.C# => executable object.GetHtml(model)

            return html;
        }

        private object GetInstance(string viewCSharpCode, string typeName, Type viewModelType)
        {
            var tempFileName = Path.GetRandomFileName() + ".dll";
            var compilation = CSharpCompilation.Create(tempFileName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IEnumerable<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location))
                .AddReferences(MetadataReference.CreateFromFile(viewModelType.Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(viewCSharpCode));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Console.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = Assembly.Load(ms.ToArray());
                var viewType = assembly.GetType(typeName);

                return Activator.CreateInstance(viewType);
            }
        }

        private string GenerateCSharpMethodBody(string viewCode)
        {
            return "";
        }

    }
}
