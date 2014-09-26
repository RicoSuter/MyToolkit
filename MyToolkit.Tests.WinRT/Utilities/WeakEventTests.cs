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
        public class MyTargetClass
        {
            public Action Action { get; set; }

            public MyTargetClass(Action action)
            {
                Action = action; 
            }

            public void Callback(object sender, PropertyChangedEventArgs args)
            {
                Action();
            }
        }

        [TestMethod]
        public void When_gc_collects_then_event_should_be_deregistered_before_event_call()
        {
            //// Arrange
            var eventCalled = false; 
            var target = new MyTargetClass(() =>
            {
                eventCalled = true; 
            });
            var obj = new MyObject();

            EventUtilities.RegisterWeakEvent<MyObject, PropertyChangedEventArgs>(obj, "PropertyChanged", target.Callback);

            //// Act
            obj.Name = "test1";
            Assert.IsTrue(eventCalled);
            target = null;

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
