//-----------------------------------------------------------------------
// <copyright file="EventHandlerExtensionsTests.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyToolkit.Utilities;

namespace MyToolkit.Tests.Wpf45.Utilities
{
    [TestClass]
    public class EventHandlerExtensionsTests
    {
        [TestMethod]
        public void When_raising_event_then_callback_should_be_called()
        {
            //// Arrange
            var count = 0; 
            var obj = new EventHandlerTestStub();
            obj.PropertyChanged += delegate { count++; };

            //// Act
            obj.RaiseEvent();

            //// Assert
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void When_no_callback_is_registered_then_raise_should_not_throw_error()
        {
            //// Arrange
            var obj = new EventHandlerTestStub();

            //// Act
            obj.RaiseEvent();

            //// Assert
        }
    }

    public class EventHandlerTestStub : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaiseEvent()
        {
            PropertyChanged.Raise(null, new PropertyChangedEventArgs("test"));
        }
    }
}
