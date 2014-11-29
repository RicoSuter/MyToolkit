using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Collections;
using MyToolkit.Model;

namespace MyToolkit.Tests.WinRT.Collections
{
    [TestClass]
    public class ObservableCollectionViewTests
    {
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

        [TestMethod]
        public void When_adding_an_element_then_filter_should_work()
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
            view.Filter = c => c.First == "a";

            source.Add(new SampleClass { First = "a" });

            //// Assert
            Assert.AreEqual(3, view.Count);
        }

        [TestMethod]
        public void When_changing_element_then_view_should_update()
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
            view.TrackItemChanges = true;
            view.Filter = c => c.First == "a";

            var item = source.First();
            item.First = "b";

            //// Assert
            Assert.AreEqual(1, view.Count);
        }

        [TestMethod]
        public void When_adding_item_then_change_tracking_should_work()
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
            view.TrackItemChanges = true;
            view.Filter = c => c.First == "a";

            var item = new SampleClass { First = "b", Last = "c" };
            source.Add(item);

            item.First = "a";

            //// Assert
            Assert.AreEqual(3, view.Count);
        }

        [TestMethod]
        public void When_adding_item_by_initialize_then_change_tracking_should_work()
        {
            //// Arrange
            var source = new MtObservableCollection<SampleClass>
            {
                new SampleClass {First = "a", Last = "b"},
                new SampleClass {First = "a", Last = "b"},
                new SampleClass {First = "b", Last = "c"},
                new SampleClass {First = "b", Last = "c"},
            };

            //// Act
            var view = new ObservableCollectionView<SampleClass>(source);
            view.TrackItemChanges = true;
            view.Filter = c => c.First == "a";

            var item = new SampleClass { First = "b", Last = "c" };
            var copy = source.ToList();
            copy.Add(item);

            source.Initialize(copy);

            item.First = "a";

            //// Assert
            Assert.AreEqual(3, view.Count);
        }
    }

    public class SampleClass : ObservableObject
    {
        private string _first;

        public string First
        {
            get { return _first; }
            set { Set(ref _first, value); }
        }

        public string Last { get; set; }
    }
}
