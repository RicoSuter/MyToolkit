using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Composition;

namespace MyToolkit.Tests.WinRT.Composition
{
    [TestClass]
    public class ServiceLocatorTests
    {
        public interface IService
        {
            int DoIt();
        }

        public class ServiceImplementation : IService
        {
            public int DoIt()
            {
                return 10;
            }
        }

        [TestMethod]
        public void When_registering_lazy_service_then_they_should_be_accessible()
        {
            //// Arrange
            ServiceLocator.Default.RegisterSingleton<IService, ServiceImplementation>();

            //// Act
            var service = ServiceLocator.Default.Resolve<IService>();

            //// Assert
            Assert.AreEqual(10, service.DoIt());
        }

        [TestMethod]
        public void When_registering_service_then_they_should_be_accessible()
        {
            //// Arrange
            ServiceLocator.Default.RegisterSingleton<IService, ServiceImplementation>(new ServiceImplementation());

            //// Act
            var service = ServiceLocator.Default.Resolve<IService>();

            //// Assert
            Assert.AreEqual(10, service.DoIt());
        }
    }
}
