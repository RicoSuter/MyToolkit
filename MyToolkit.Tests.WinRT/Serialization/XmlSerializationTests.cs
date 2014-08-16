using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Serialization;
using MyToolkit.Tests.Model;

namespace MyToolkit.Tests.Serialization
{
    [TestClass]
    public class XmlSerializationTests
    {
        [TestMethod]
        public void When_serializing_an_object_then_round_trip_should_work()
        {
            //// Arrange
            var obj = new MyObject();
            obj.Name = "a";

            //// Act
            var xml = XmlSerialization.Serialize(obj);
            var obj2 = XmlSerialization.Deserialize<MyObject>(xml);
            var xml2 = XmlSerialization.Serialize(obj2); 

            //// Assert
            Assert.AreEqual(xml, xml2);
        }

        [TestMethod]
        public void When_creating_a_serializer_then_it_should_be_cached()
        {
            //// Act
            var serializer1 = XmlSerialization.CreateSerializer<MyObject>();
            var serializer2 = XmlSerialization.CreateSerializer<MyObject>();
            var serializer3 = XmlSerialization.CreateSerializer<MyObject>(new Type[] { typeof(MyObject) });
            var serializer4 = XmlSerialization.CreateSerializer<MyObject>(new Type[] { typeof(MyObject) });

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
            var serializer1 = XmlSerialization.CreateSerializer<MyObject>(null, false);
            var serializer2 = XmlSerialization.CreateSerializer<MyObject>(null, false);
            var serializer3 = XmlSerialization.CreateSerializer<MyObject>(new Type[] { typeof(MyObject) }, false);
            var serializer4 = XmlSerialization.CreateSerializer<MyObject>(new Type[] { typeof(MyObject) }, false);

            //// Assert
            Assert.AreNotEqual(serializer1, serializer2);
            Assert.AreNotEqual(serializer3, serializer4);

            Assert.AreNotEqual(serializer1, serializer3);
            Assert.AreNotEqual(serializer2, serializer4);
        }
    }
}
