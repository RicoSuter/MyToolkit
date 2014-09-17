using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Collections;
using MyToolkit.Model;
using MyToolkit.Tests.WinRT.Model;

namespace MyToolkit.Tests.WinRT.Collections
{
    [TestClass]
    public class ExtendedObservableCollectionTests
    {
        private static IReadOnlyList<string> _addedItems;
        private static IReadOnlyList<string> _removedItems;
        private static ExtendedObservableCollection<string> _collection;

        [TestMethod]
        public void When_item_removed_then_event_is_triggered_with_it()
        {
            //// Arrange
            Given_a_collection_with_multiple_items();

            //// Act
            _collection.Remove("b");

            //// Assert
            Assert.AreEqual(3, _collection.Count);
            Assert.AreEqual(0, _addedItems.Count);
            Assert.AreEqual(1, _removedItems.Count);
            Assert.AreEqual("b", _removedItems[0]);
        }

        [TestMethod]
        public void When_clearing_items_then_event_must_contain_all_items()
        {
            //// Arrange
            Given_a_collection_with_multiple_items();

            //// Act
            _collection.Clear();

            //// Assert
            Assert.AreEqual(0, _collection.Count);
            Assert.AreEqual(0, _addedItems.Count);
            Assert.AreEqual(4, _removedItems.Count);
        }

        private static void Given_a_collection_with_multiple_items()
        {
            _addedItems = new List<string>();
            _removedItems = new List<string>();
            _collection = new ExtendedObservableCollection<string>
            {
                "a",
                "b",
                "c",
                "d"
            };
            _collection.ExtendedCollectionChanged += (sender, args) =>
            {
                _addedItems = args.AddedItems;
                _removedItems = args.RemovedItems;
            };
        }

        [TestMethod]
        public void When_changing_child_in_list_then_event_should_be_triggered()
        {
            //// Arrange
            var list = new List<string>();

            var child = new MyObject();
            var obj = new MyObject();
            obj.List.Add(child);
            obj.GraphPropertyChanged += (sender, args) => list.Add(args.PropertyName);

            //// Act
            child.Name = "A";

            //// Assert
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Contains("Name"));
            Assert.IsTrue(list.Contains("FullName"));
        }

        [TestMethod]
        public void When_changing_a_collection_then_event_should_be_triggered()
        {
            //// Arrange
            var list = new List<PropertyChangedEventArgs>();

            var child = new MyObject();
            var obj = new MyObject();
            obj.List.Add(child);
            obj.GraphPropertyChanged += (sender, args) => list.Add(args);

            //// Act
            obj.List.Add(child);

            //// Assert
            Assert.AreEqual(1, list.OfType<IExtendedNotifyCollectionChangedEventArgs>().Count());
        }

        [TestMethod]
        public void When_changing_child_of_property_then_event_should_be_triggered()
        {
            //// Arrange
            var list = new List<string>();

            var child = new MyObject();
            var obj = new MyObject();
            obj.Child = child;
            obj.GraphPropertyChanged += (sender, args) => list.Add(args.PropertyName);

            //// Act
            child.Name = "B";

            //// Assert
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Contains("Name"));
            Assert.IsTrue(list.Contains("FullName"));
        }

        [TestMethod]
        public void When_changing_a_property_then_old_value_should_be_provided()
        {
            //// Arrange
            var list = new List<PropertyChangedEventArgs>();

            var obj = new MyObject();
            obj.Name = "A";
            obj.GraphPropertyChanged += (sender, args) => list.Add(args);

            //// Act
            obj.Name = "B";

            //// Assert
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("A", ((ExtendedPropertyChangedEventArgs)list[0]).OldValue);
            Assert.AreEqual("B", ((ExtendedPropertyChangedEventArgs)list[0]).NewValue);
        }

        [TestMethod]
        public void When_changing_property_in_cyclic_graph_then_event_should_occur_only_once()
        {
            //// Arrange
            var list = new List<PropertyChangedEventArgs>();

            var objA = new MyObject();
            objA.Name = "A";

            var objB = new MyObject();
            objB.Name = "B";
            objB.Child = objA;
            objA.Child = objB;

            objA.GraphPropertyChanged += (sender, args) => list.Add(args);

            //// Act
            objB.Name = "C";

            //// Assert
            Assert.AreEqual(2, list.Count);
        }
    }
}
