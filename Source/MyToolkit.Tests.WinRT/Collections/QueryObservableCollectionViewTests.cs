using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Collections;

namespace MyToolkit.Tests.WinRT.Collections
{
    [TestClass]
    public class QueryObservableCollectionViewTests
    {
        [TestMethod]
        public void When_filtering_query_collection_then_count_should_be_correct()
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
            var view = new QueryObservableCollectionView<SampleClass>(source);
            Assert.AreEqual(4, view.Count);
            view.Query = list => list.Where(s => s.First == "a");

            //// Assert
            Assert.AreEqual(2, view.Count);
        }
    }
}