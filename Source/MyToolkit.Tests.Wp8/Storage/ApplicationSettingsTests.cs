using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Storage;

namespace MyToolkit.Tests.Wp8.Storage
{
    [TestClass]
    public class ApplicationSettingsTests
    {
        [TestMethod]
        public void When_storing_setting_then_it_should_be_retrievable()
        {
            //// Arrange
            ApplicationSettings.SetSetting("string", "A");
            ApplicationSettings.SetSetting("integer", 1);
            ApplicationSettings.SetSetting("double", 1.0);

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
            ApplicationSettings.SetSetting("string", "A");
            ApplicationSettings.SetSetting("integer", 1);
            ApplicationSettings.SetSetting("double", 1.0);

            //// Act
            Assert.IsTrue(ApplicationSettings.HasSetting<string>("string"));
            Assert.IsTrue(ApplicationSettings.HasSetting<int>("integer"));
            Assert.IsTrue(ApplicationSettings.HasSetting<double>("double"));

            ApplicationSettings.RemoveSetting("string");
            ApplicationSettings.RemoveSetting("integer");
            ApplicationSettings.RemoveSetting("double");
            
            //// Assert
            Assert.IsFalse(ApplicationSettings.HasSetting<string>("string"));
            Assert.IsFalse(ApplicationSettings.HasSetting<int>("integer"));
            Assert.IsFalse(ApplicationSettings.HasSetting<double>("double"));
        }
    }
}
