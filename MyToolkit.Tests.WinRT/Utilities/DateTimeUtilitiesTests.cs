using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Utilities;

namespace MyToolkit.Tests.WinRT.Utilities
{
    [TestClass]
    public class DateTimeUtilitiesTests
    {
        [TestMethod]
        public void When_converting_unix_one_hour_then_1970_plus_one_hour_should_result()
        {
            //// Arrange
            var unix = 60 * 60;

            //// Act
            var time = DateTimeUtilities.FromUnixTimeStamp(unix);

            //// Assert
            Assert.AreEqual(1970, time.Year);
            Assert.AreEqual(1, time.Month);
            Assert.AreEqual(1, time.Day);
            Assert.AreEqual(1, time.Hour);
            Assert.AreEqual(0, time.Minute);
            Assert.AreEqual(0, time.Second);
        }

        [TestMethod]
        public void When_converting_to_unix_timestamp_and_back_then_the_same_value_should_result()
        {
            //// Arrange
            var time = DateTime.Now;

            //// Act
            var unix = time.ToUnixTimeStamp();
            var time2 = DateTimeUtilities.FromUnixTimeStamp(unix);
            var unix2 = time2.ToUnixTimeStamp();
            
            //// Assert
            Assert.AreEqual(unix, unix2, 0.005);
        }
    }
}
