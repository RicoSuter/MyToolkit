using System;
using Windows.UI.Xaml;
using MyToolkit.Model;

namespace SampleUwpApp
{
    public class HamburgerItem : DependencyObject
    {
        public HamburgerItem()
        {
            FindPageType = true; 
        }

        public string Icon { get; set; }

        public Type PageType { get; set; }

        public object PageParameter { get; set; }

        /// <summary>Gets or sets a value indicating whether to search for page in the current page stack (default: true).</summary>
        public bool FindPageType { get; set; }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(HamburgerItem), new PropertyMetadata(default(object)));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof (bool), typeof (HamburgerItem), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
    }
}
