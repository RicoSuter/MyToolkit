using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Model;

namespace MyToolkit.Tests.WinRT.Model
{
    [TestClass]
    public class AsyncValidatedObservableObjectTests
    {
        [TestMethod]
        public void When_validating_async_then_property_validation_is_async()
        {
            //// Arrange
            var person = new AsyncPerson();
            person.AsyncValidation = true;
            person.PropertyBlocks = true; 

            //// Act
            person.Name = "foobar";

            //// Assert
            Assert.AreEqual(null, person.GetErrors("Name"));
        }

        [TestMethod]
        public void When_validating_async_then_invariant_validation_is_async()
        {
            //// Arrange
            var person = new AsyncPerson();
            person.AsyncValidation = true;
            person.InvariantBlocks = true;

            //// Act
            person.Name = "foobar";

            //// Assert
            Assert.AreEqual(null, person.GetErrors("Name"));
        }

        [TestMethod]
        public void When_validating_not_async_then_property_validation_blocks()
        {
            //// Arrange
            var person = new AsyncPerson();
            person.AsyncValidation = false;
            person.PropertyBlocks = true; 

            //// Act
            person.Name = "foobar";

            //// Assert
            Assert.AreEqual(1, person.GetErrors("Name").Count());
        }

        [TestMethod]
        public void When_validating_not_async_then_invariant_validation_blocks()
        {
            //// Arrange
            var person = new AsyncPerson();
            person.AsyncValidation = false; 
            person.InvariantBlocks = true; 

            //// Act
            person.Name = "foobar";

            //// Assert
            Assert.AreEqual(1, person.GetErrors("Name").Count());
        }

        public class AsyncPerson : AsyncValidatedObservableObject
        {
            private string _name;

            [StringLength(3)]
            public string Name
            {
                get
                {
                    if (PropertyBlocks)
                        Task.Delay(1000).Wait();
                    return _name;
                }
                set { Set(ref _name, value); }
            }


            public bool HasInvariantError { get; set; }

            public bool InvariantBlocks { get; set; }

            public bool PropertyBlocks { get; set; }

            /// <summary>Validates the invariants.</summary>
            /// <returns>The list of validation errors (never <c>null</c>). </returns>
            protected override ICollection<string> ValidateInvariants()
            {
                if (InvariantBlocks)
                    Task.Delay(1000).Wait();

                var errors = base.ValidateInvariants();
                if (HasInvariantError)
                    errors.Add("Foo");

                return errors;
            }
        }
    }
}