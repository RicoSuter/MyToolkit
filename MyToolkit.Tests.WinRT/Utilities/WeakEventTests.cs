using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Tests.WinRT.Model;
using MyToolkit.Utilities;

namespace MyToolkit.Tests.WinRT.Utilities
{
    [TestClass]
    public class WeakEventTests
    {
        [TestMethod]
        public void When_register_weak_event_then_it_should_get_correctly_called()
        {
            //// Arrange
            var obj = new MyObject();
            var eventCalled = false;
            var correctObject = false; 

            WeakEvent.Register<WeakEventTests, PropertyChangedEventHandler, PropertyChangedEventArgs>(
                this,
                h => obj.PropertyChanged += h,
                h => obj.PropertyChanged -= h,
                h => (o, e) => h(o, e),
                (subscriber, sender, args) =>
                {
                    correctObject = sender == obj;
                    eventCalled = true;
                });

            //// Act
            obj.Name = "test";

            //// Assert
            Assert.IsTrue(eventCalled);
            Assert.IsTrue(correctObject);
        }

        [TestMethod]
        public void When_gc_collects_then_event_should_be_deregistered_before_event_call()
        {
            //// Arrange
            var subs = new WeakEventTests();
            var obj = new MyObject();
            var eventCalled = false;

            WeakEvent.Register<WeakEventTests, PropertyChangedEventHandler, PropertyChangedEventArgs>(
                subs,
                h => obj.PropertyChanged += h,
                h => obj.PropertyChanged -= h,
                h => (o, e) => h(o, e),
                (subscriber, sender, args) => { eventCalled = true; });

            //// Act
            obj.Name = "test1";
            Assert.IsTrue(eventCalled);
            subs = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            eventCalled = false;
            obj.Name = "test2"; 

            //// Assert
            Assert.IsFalse(eventCalled);
        }
    }
}
