using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Model;

namespace MyToolkit.Tests.WinRT.Model
{
    [TestClass]
    public class ValidatedObservableObjectTests
    {
        [TestMethod]
        public void When_property_has_error_than_object_automatically_has_errors()
        {
            //// Arrange
            var person = new Person(); 

            //// Act
            person.Name = "foobar"; 

            //// Assert
            Assert.IsTrue(person.HasErrors);
            Assert.AreEqual(1, person.GetErrors("Name").Count());
        }

        [TestMethod]
        public void When_object_has_invariant_error_than_has_errors_after_validation()
        {
            //// Arrange
            var person = new Person();

            //// Act
            person.HasInvariantError = true; 
            person.Validate();

            //// Assert
            Assert.IsTrue(person.HasErrors);
            Assert.AreEqual(1, person.InvariantErrors.Count());
        }

        [TestMethod]
        public void When_property_has_error_than_object_has_errors_after_validation()
        {
            //// Arrange
            var person = new Person();
            person.AutoValidateProperties = false; 

            //// Act
            person.Name = "foobar";
            person.Validate();

            //// Assert
            Assert.IsTrue(person.HasErrors);
            Assert.AreEqual(1, person.GetErrors("Name").Count());
        }

        [TestMethod]
        public void When_property_has_error_and_auto_validation_is_disabled_than_object_has_no_errors()
        {
            //// Arrange
            var person = new Person();
            person.AutoValidateProperties = false;

            //// Act
            person.Name = "foobar";

            //// Assert
            Assert.IsFalse(person.HasErrors);
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

            public bool HasInvariantError { get; set; }

            /// <summary>Validates the invariants.</summary>
            /// <returns>The list of validation errors (never <c>null</c>). </returns>
            protected override ICollection<string> ValidateInvariants()
            {
                var errors = base.ValidateInvariants();
                if (HasInvariantError)
                    errors.Add("Foo");
                return errors;
            }
        }
    }
}