using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
    internal class HamburgerItemDataDto : DependencyObject
    {
        public static readonly DependencyProperty SplitViewProperty = DependencyProperty.Register(
            "SplitView", typeof (object), typeof (HamburgerItemDataDto), new PropertyMetadata(default(object)));

        public object SplitView
        {
            get { return (object) GetValue(SplitViewProperty); }
            set { SetValue(SplitViewProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof (string), typeof (HamburgerItemDataDto), new PropertyMetadata(default(string)));

        public string Icon
        {
            get { return (string) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}