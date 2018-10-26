﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
            string viewCSharpCode = $@"
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SIS.MvcFramework.ViewEngine;
using SIS.MvcFramework.ViewEngine.Contracts;
using {typeof(T).Namespace};
namespace MyAppView
{{
    public class {viewTypeName} : IView<{typeof(T).FullName.Replace("+", ".")}>
    {{
        public string GetHtml({typeof(T).FullName.Replace("+", ".")} model)
        {{
            var html = new StringBuilder();

            var Model = model;

            {csharpMethodBody}

            string result = html.ToString().TrimEnd();
            
            return result;
        }}
    }}
}}";
            var instanceOfViewClass = GetInstance(viewCSharpCode, $"MyAppView.{viewTypeName}", typeof(T)) as IView<T>;

            var html = instanceOfViewClass.GetHtml(model);

            //2.C# => executable object.GetHtml(model)

            return html;
        }

        private object GetInstance(string viewCSharpCode, string typeName, Type viewModelType)
        {
            var compilation = CSharpCompilation.Create(Path.GetRandomFileName() + ".dll")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IEnumerable<>).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(viewModelType.Assembly.Location))
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(viewCSharpCode));

            var netstandardReferences = Assembly.Load(new AssemblyName("netstandard")).GetReferencedAssemblies();
            foreach (var netstandardReference in netstandardReferences)
            {
                compilation = compilation.AddReferences(
                    MetadataReference.CreateFromFile(Assembly.Load(netstandardReference).Location));
            }

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
                var instance = Activator.CreateInstance(viewType);

                return instance;
            }
        }

        private string GenerateCSharpMethodBody(string viewCode)
        {
            var lines = GetLines(viewCode);
            var sb = new StringBuilder();
            
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("{") || line.Trim().StartsWith("}")
                    || line.Trim().StartsWith("@"))
                {
                    var returnedLine = GetCodeLine(line);
                    sb.Append(returnedLine).Append(Environment.NewLine);
                }
                else
                {
                    var htmlLine = line;
                    var pattern = @"@(?<value>[^< ]+)";
                    var regex = new Regex(pattern);
                    var matches = regex.Matches(htmlLine).ToArray();
                    if (matches.Length > 0)
                    {
                        for (int i = 0; i < matches.Length; i++)
                        {
                            var stringToReplace = matches[i].Groups[0].ToString();
                            var newString = matches[i].Groups["value"].ToString();
                            var index = htmlLine.IndexOf(stringToReplace);
                            htmlLine = $@"{htmlLine.Substring(0,index).Replace("\"", "\\\"")}{{{newString.ToString()}}}{htmlLine.Substring(index + stringToReplace.Length).Replace("\"", "\\\"")}";
                        }
                    }
                    else
                    {
                        htmlLine = htmlLine.Replace("\"", "\\\"");
                    }
                    
                    var lineToAppend = $"html.Append($\"{htmlLine}\").Append(Environment.NewLine);";

                    sb.Append(lineToAppend).Append(Environment.NewLine);
                }
            }

            var result = sb.ToString().TrimEnd();

            return result;
        }

        private static string GetCodeLine(string line)
        {
            if (line.Trim().StartsWith("@"))
            {
                var index = line.IndexOf("@", StringComparison.Ordinal);
                line = line.Remove(index, 1);
            }

            return line;
        }

        private IEnumerable<string> GetLines(string viewCode)
        {
            var lines = new List<string>();
            var stringReader = new StringReader(viewCode);

            var line = string.Empty;

            while ((line = stringReader.ReadLine()) != null)
            {
                lines.Add(line);
            }

            return lines;
        }
    }
}
