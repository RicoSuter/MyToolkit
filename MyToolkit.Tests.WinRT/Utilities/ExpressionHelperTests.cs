using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Messaging;
using MyToolkit.Utilities;

namespace MyToolkit.Tests.WinRT.Utilities
{
    [TestClass]
    public class ExpressionHelperTests
    {
        [TestMethod]
        public void When_getting_name_then_no_exception_should_be_thrown()
        {
            Assert.AreEqual("Text", ExpressionHelper.GetName<TextMessage, string>(i => i.Text));
            Assert.AreEqual("Text", ExpressionHelper.GetName<TextMessage>(i => i.Text));
        }
    }
}
