using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Model;

namespace MyToolkit.Tests.WinRT.Model
{
    [TestClass]
    public class ValidatedObservableObjectTests
    {
        [TestMethod]
        public void When_property_has_error_than_object_has_errors()
        {
            //// Arrange
            var person = new Person(); 

            //// Act
            person.Name = "foobar"; 

            //// Assert
            Assert.IsTrue(person.HasErrors);
            Assert.AreEqual(1, person.GetErrors("Name").Count);
        }
    }

    public class Person : ValidatedObservableObject
    {
        private string _name;

        [StringLength(3)]
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }
    }
}