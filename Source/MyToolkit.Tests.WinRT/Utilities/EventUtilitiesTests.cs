using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Events;
using MyToolkit.Tests.WinRT.Mocks;
using MyToolkit.Utilities;

namespace MyToolkit.Tests.WinRT.Utilities
{
    [TestClass]
    public class EventUtilitiesTests
    {
        //[TestMethod]
        //public async Task Test()
        //{
        //    //// Arrange
        //    var obj = new MockObservableObject();
        //    var eventCalled = false;
        //    var target = new MockEventTargetClass(() =>
        //    {
        //        eventCalled = true;
        //    });

        //    //// Act
        //    await EventUtilities.WaitForEventAsync<MockObservableObject, PropertyChangedEventHandler, PropertyChangedEventArgs>(
        //        obj,
        //        (o, h) => o.PropertyChanged += h,
        //        (o, h) => o.PropertyChanged -= h);

        //    obj.Name = "test1";
        //    Assert.IsTrue(eventCalled);
        //    target = null;

        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    GC.Collect();

        //    eventCalled = false;
        //    obj.Name = "test2";

        //    //// Assert
        //    Assert.IsFalse(eventCalled);
        //}

        [TestMethod]
        public void When_registering_weak_event_then_it_should_be_removed_when_target_is_gone()
        {
            //// Arrange
            var obj = new MockObservableObject();
            var eventCalled = false;
            var target = new MockEventTargetClass(() =>
            {
                eventCalled = true;
            });

            //// Act
            WeakEvent.RegisterEvent<MockObservableObject, PropertyChangedEventArgs>(obj, "PropertyChanged", target.Callback);

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

        [TestMethod]
        public void When_registering_weak_event_then_it_should_still_be_here_when_target_referenced()
        {
            //// Arrange
            var eventCalled = false;
            var target = new MockEventTargetClass(() =>
            {
                eventCalled = true;
            });
            var obj = new MockObservableObject();

            //// Act
            WeakEvent.RegisterEvent<MockObservableObject, PropertyChangedEventArgs>(obj, "PropertyChanged", target.Callback);

            obj.Name = "test1";
            Assert.IsTrue(eventCalled);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            eventCalled = false;
            obj.Name = "test2";

            //// Assert
            Assert.IsTrue(eventCalled);
            target.Action = null;
        }

        [TestMethod]
        public void When_registering_typed_weak_event_then_it_should_be_removed_when_target_is_gone()
        {
            //// Arrange
            var eventCalled = false;
            var target = new MockEventTargetClass(() =>
            {
                eventCalled = true;
            });
            var obj = new MockObservableObject();

            //// Act
            WeakEvent.RegisterEvent<MockEventTargetClass, PropertyChangedEventHandler, PropertyChangedEventArgs>(
                target,
                h => obj.PropertyChanged += h,
                h => obj.PropertyChanged -= h,
                h => (o, e) => h(o, e),
                (t, sender, args) => t.Callback(sender, args));

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

        [TestMethod]
        public void When_registering_typed_weak_event_then_it_should_still_be_here_when_target_referenced()
        {
            //// Arrange
            var eventCalled = false;
            var target = new MockEventTargetClass(() =>
            {
                eventCalled = true;
            });
            var obj = new MockObservableObject();

            //// Act
            WeakEvent.RegisterEvent<MockEventTargetClass, PropertyChangedEventHandler, PropertyChangedEventArgs>(
                target,
                h => obj.PropertyChanged += h,
                h => obj.PropertyChanged -= h,
                h => (o, e) => h(o, e),
                (t, sender, args) => t.Callback(sender, args));

            obj.Name = "test1";
            Assert.IsTrue(eventCalled);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            eventCalled = false;
            obj.Name = "test2";

            //// Assert
            Assert.IsTrue(eventCalled);
            target.Action = null;
        }
    }
}
