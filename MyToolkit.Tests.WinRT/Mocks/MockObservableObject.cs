using System;
using System.Collections.ObjectModel;
using MyToolkit.Model;

namespace MyToolkit.Tests.WinRT.Mocks
{
    public class MockObservableObject : ExtendedObservableObject
    {
        private string _name;
        private MockObservableObject _child;

        public MockObservableObject()
        {
            List = new ObservableCollection<MockObservableObject>();
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

        public MockObservableObject Child
        {
            get { return _child; }
            set { Set(ref _child, value); }
        }

        public ObservableCollection<MockObservableObject> List { get; private set; } 

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