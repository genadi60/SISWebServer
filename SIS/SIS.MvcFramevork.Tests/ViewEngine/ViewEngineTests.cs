using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SIS.MvcFramework.ViewEngine.Contracts;
using Xunit;

namespace SIS.MvcFramework.Tests.ViewEngine
{
    public class ViewEngineTests
    {
        [Theory]
        [InlineData("ifForAndForeach")]
        [InlineData("viewWithNoCode")]
        [InlineData("workWithViewModel")]
        public void RunTestViews(string testViewName)
        {
            var viewCodeLines = File.ReadAllLines($"TestViews/{testViewName}.html");
            var viewCode = new StringBuilder();
            foreach (var viewCodeLine in viewCodeLines)
            {
                viewCode.Append(viewCodeLine).Append(Environment.NewLine);
            }
            var expectedResultLines = File.ReadAllLines($"TestViews/{testViewName}.Result.html");
            var expectedResult = new StringBuilder();
            foreach (var expectedResultLine in expectedResultLines)
            {
                expectedResult.Append(expectedResultLine).Append(Environment.NewLine);
            }
            IViewEngine viewEngine = new MvcFramework.ViewEngine.ViewEngine();
            var model = new TestModel
            {
                String = "Username",
                List = new List<string>{"Item1", "item2", "test", "123", " "}
            };
            var code = viewCode.ToString().TrimEnd();
            var result = viewEngine.GetHtml(testViewName, code, model);
            var expected = expectedResult.ToString().TrimEnd();
            Assert.Equal(expected, result);
        }

        public class TestModel
        {
            public string String { get; set; }

            public IEnumerable<string> List { get; set; }
        }
    }
}
