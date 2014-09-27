using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Serialization;
using MyToolkit.Tests.WinRT.Mocks;
using MyToolkit.Tests.WinRT.Model;

namespace MyToolkit.Tests.WinRT.Serialization
{
    [TestClass]
    public class XmlSerializationTests
    {
        [TestMethod]
        public void When_serializing_an_object_then_round_trip_should_work()
        {
            //// Arrange
            var obj = new MockObservableObject();
            obj.Name = "a";

            //// Act
            var xml = XmlSerialization.Serialize(obj);
            var obj2 = XmlSerialization.Deserialize<MockObservableObject>(xml);
            var xml2 = XmlSerialization.Serialize(obj2); 

            //// Assert
            Assert.AreEqual(xml, xml2);
        }

        [TestMethod]
        public void When_creating_a_serializer_then_it_should_be_cached()
        {
            //// Act
            var serializer1 = XmlSerialization.CreateSerializer<MockObservableObject>();
            var serializer2 = XmlSerialization.CreateSerializer<MockObservableObject>();
            var serializer3 = XmlSerialization.CreateSerializer<MockObservableObject>(new Type[] { typeof(MockObservableObject) });
            var serializer4 = XmlSerialization.CreateSerializer<MockObservableObject>(new Type[] { typeof(MockObservableObject) });

            //// Assert
            Assert.AreEqual(serializer1, serializer2);
            Assert.AreEqual(serializer3, serializer4);

            Assert.AreNotEqual(serializer1, serializer3);
            Assert.AreNotEqual(serializer2, serializer4);
        }

        [TestMethod]
        public void When_creating_a_serializer_without_caching_then_it_should_not_be_cached()
        {
            //// Act
            var serializer1 = XmlSerialization.CreateSerializer<MockObservableObject>(null, false);
            var serializer2 = XmlSerialization.CreateSerializer<MockObservableObject>(null, false);
            var serializer3 = XmlSerialization.CreateSerializer<MockObservableObject>(new Type[] { typeof(MockObservableObject) }, false);
            var serializer4 = XmlSerialization.CreateSerializer<MockObservableObject>(new Type[] { typeof(MockObservableObject) }, false);

            //// Assert
            Assert.AreNotEqual(serializer1, serializer2);
            Assert.AreNotEqual(serializer3, serializer4);

            Assert.AreNotEqual(serializer1, serializer3);
            Assert.AreNotEqual(serializer2, serializer4);
        }
    }
}
