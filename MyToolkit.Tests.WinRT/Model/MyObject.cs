using System;
using System.Collections.ObjectModel;
using MyToolkit.Model;

namespace MyToolkit.Tests.WinRT.Model
{
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