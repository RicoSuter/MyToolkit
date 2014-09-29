using System.ComponentModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Messaging;
using MyToolkit.Utilities;

namespace MyToolkit.Tests.WinRT.Utilities
{
    [TestClass]
    public class ExpressionUtilitiesTests
    {
        [TestMethod]
        public void When_getting_property_name_then_no_exception_should_be_thrown()
        {
            Assert.AreEqual("Text", ExpressionUtilities.GetPropertyName<TextMessage, string>(i => i.Text));
            Assert.AreEqual("Text", ExpressionUtilities.GetPropertyName<TextMessage>(i => i.Text));
        }

        //[TestMethod]
        //public void When_getting_event_name_then_no_exception_should_be_thrown()
        //{
        //    Assert.AreEqual("PropertyChanged", ExpressionUtilities.GetEventName<INotifyPropertyChanged>(i => i.PropertyChanged -= null));
        //}
    }
}
