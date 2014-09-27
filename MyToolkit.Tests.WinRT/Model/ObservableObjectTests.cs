using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Tests.WinRT.Mocks;

namespace MyToolkit.Tests.WinRT.Model
{
    [TestClass]
    public class ObservableObjectTests
    {
        [TestMethod]
        public void When_changing_property_then_event_should_be_triggered()
        {
            //// Arrange
            var list = new List<string>();
            var obj = new MockObservableObject();
            obj.PropertyChanged += (sender, args) => list.Add(args.PropertyName);

            //// Act
            obj.Name = "A";

            //// Assert
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Contains("Name"));
            Assert.IsTrue(list.Contains("FullName"));
        }

        [TestMethod]
        public void When_raising_property_changed_event_externally_then_event_should_be_triggered()
        {
            //// Arrange
            var list = new List<string>();
            var obj = new MockObservableObject();
            obj.PropertyChanged += (sender, args) => list.Add(args.PropertyName);

            //// Act
            obj.RaisePropertyChanged<MockObservableObject>(i => i.Name);

            //// Assert
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list.Contains("Name"));
        }
    }
}
