using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyToolkit.Model;

namespace MyToolkit.Tests.Model
{
    [TestClass]
    public class ObservableObjectTests
    {
        [TestMethod]
        public void When_changing_property_then_event_should_be_triggered()
        {
            //// Arrange
            var list = new List<string>();
            var obj = new MyObject();
            obj.PropertyChanged += (sender, args) => list.Add(args.PropertyName);

            //// Act
            obj.Name = "A";

            //// Assert
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Contains("Name"));
            Assert.IsTrue(list.Contains("FullName"));
        }

        [TestMethod]
        public void When_raising_property_changed_event_externally_then_event_should_be_triggered()
        {
            //// Arrange
            var list = new List<string>();
            var obj = new MyObject();
            obj.PropertyChanged += (sender, args) => list.Add(args.PropertyName);

            //// Act
            obj.RaisePropertyChanged<MyObject>(i => i.Name);

            //// Assert
            Assert.AreEqual(1, list.Count);
            Assert.IsTrue(list.Contains("Name"));
        }
    }

    public class MyObject : ExtendedObservableObject
    {
        private string _name;
        private MyObject _child;

        public MyObject()
        {
            List = new ObservableCollection<MyObject>();
            RegisterChild(List);
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (Set(ref _name, value))
                    RaisePropertyChanged(() => FullName);
            }
        }

        public MyObject Child
        {
            get { return _child; }
            set { Set(ref _child, value); }
        }

        public ObservableCollection<MyObject> List { get; private set; } 

        public string FullName
        {
            get { return Name + "Test"; }
        }

        public void AddExcludedChildType(Type childType)
        {
            ExcludedChildTypes.Add(childType);
        }
    }
}
