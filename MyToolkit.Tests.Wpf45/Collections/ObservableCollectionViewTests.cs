using System;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyToolkit.Collections;

namespace MyToolkit.Tests.Wpf45.Collections
{
    [TestClass]
    public class ObservableCollectionViewTests
    {
        public class SampleClass
        {
            public string First { get; set; }
            public string Last { get; set; }
        }

        [TestMethod]
        public void When_filtering_collection_then_count_should_be_correct()
        {
            //// Arrange
            var source = new ObservableCollection<SampleClass>
            {
                new SampleClass {First = "a", Last = "b"},
                new SampleClass {First = "a", Last = "b"},
                new SampleClass {First = "b", Last = "c"},
                new SampleClass {First = "b", Last = "c"},
            };

            //// Act
            var view = new ObservableCollectionView<SampleClass>(source);
            Assert.AreEqual(4, view.Count);
            view.Filter = c => c.First == "a";

            //// Assert
            Assert.AreEqual(2, view.Count);
        }
    }
}
