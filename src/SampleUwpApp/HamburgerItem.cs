using System;
using MyToolkit.Model;

namespace SampleUwpApp
{
    public class HamburgerItem : ObservableObject
    {
        private bool _isSelected;

        public string Icon { get; set; }

        public string Label { get; set; }

        public Type PageType { get; set; }

        public object PageParameter { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            internal set { Set(ref _isSelected, value); }
        }
    }
}
