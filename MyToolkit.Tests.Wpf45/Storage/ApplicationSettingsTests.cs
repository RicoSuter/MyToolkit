using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyToolkit.Storage;

namespace MyToolkit.Tests.Wpf45.Storage
{
    [TestClass]
    public class ApplicationSettingsTests
    {
        [TestMethod]
        public void When_storing_setting_then_it_should_be_retrievable()
        {
            //// Arrange
            ApplicationSettings.SetSetting("string", "A", false, true);
            ApplicationSettings.SetSetting("integer", 1, false, true);
            ApplicationSettings.SetSetting("double", 1.0, false, true);

            //// Act
            var str = ApplicationSettings.GetSetting("string", "");
            var integer = ApplicationSettings.GetSetting("integer", 0);
            var dou = ApplicationSettings.GetSetting("double", 0.0);

            //// Assert
            Assert.IsTrue(ApplicationSettings.HasSetting<string>("string"));
            Assert.IsTrue(ApplicationSettings.HasSetting<int>("integer"));
            Assert.IsTrue(ApplicationSettings.HasSetting<double>("double"));

            Assert.AreEqual("A", str);
            Assert.AreEqual(1, integer);
            Assert.AreEqual(1.0, dou);
        }

        [TestMethod]
        public void When_removing_setting_then_it_should_be_gone()
        {
            //// Arrange
            ApplicationSettings.SetSetting("string", "A", false, true);
            ApplicationSettings.SetSetting("integer", 1, false, true);
            ApplicationSettings.SetSetting("double", 1.0, false, true);

            //// Act
            Assert.IsTrue(ApplicationSettings.HasSetting<string>("string"));
            Assert.IsTrue(ApplicationSettings.HasSetting<int>("integer"));
            Assert.IsTrue(ApplicationSettings.HasSetting<double>("double"));

            ApplicationSettings.RemoveSetting("string", false, true);
            ApplicationSettings.RemoveSetting("integer", false, true);
            ApplicationSettings.RemoveSetting("double", false, true);
            
            //// Assert
            Assert.IsFalse(ApplicationSettings.HasSetting<string>("string"));
            Assert.IsFalse(ApplicationSettings.HasSetting<int>("integer"));
            Assert.IsFalse(ApplicationSettings.HasSetting<double>("double"));
        }
    }
}
