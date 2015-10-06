//-----------------------------------------------------------------------
// <copyright file="CodeContractExtensionsTests.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Utilities;

namespace MyToolkit.Tests.WinRT.Utilities
{
    [TestClass]
    public class CodeContractExtensionsTests
    {
        [TestMethod]
        public void When_method_with_not_nullable_is_called_then_exception_is_thrown()
        {
            try
            {
                CodeContractExtensionsTestClass.Foo(null, "Test", 1);
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual("a", exception.ParamName);
                return;
            }
            Assert.Fail("ArgumentNullException not thrown. ");
        }

        [TestMethod]
        public void When_method_with_empty_string_is_called_then_exception_is_thrown()
        {
            try
            {
                CodeContractExtensionsTestClass.Foo(new object(), string.Empty, 1);
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual("b", exception.ParamName);
                return;
            }
            Assert.Fail("ArgumentNullException not thrown. ");
        }

        [TestMethod]
        public void When_method_with_nullable_null_is_called_then_exception_is_thrown()
        {
            try
            {
                CodeContractExtensionsTestClass.Foo(new object(), "Test", null);
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual("c", exception.ParamName);
                return;
            }
            Assert.Fail("ArgumentNullException not thrown. ");
        }
    }

    public class CodeContractExtensionsTestClass
    {
        public static void Foo(object a, string b, int? c)
        {
            a.CheckNotNull("a");
            b.CheckNotNullOrEmpty("b");
            c.CheckNotNull("c");
        }
    }
}
