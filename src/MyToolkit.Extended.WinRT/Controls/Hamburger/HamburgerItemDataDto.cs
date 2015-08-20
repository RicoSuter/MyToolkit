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
            "Icon", typeof (object), typeof (HamburgerItemDataDto), new PropertyMetadata(default(object)));

        public object Icon
        {
            get { return (object) GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty ContentIconProperty = DependencyProperty.Register(
            "ContentIcon", typeof (object), typeof (HamburgerItemDataDto), new PropertyMetadata(default(object)));

        public object ContentIcon
        {
            get { return (object) GetValue(ContentIconProperty); }
            set { SetValue(ContentIconProperty, value); }
        }
    }
}