using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace MyToolkit.Controls
{
	[ContentProperty(Name = "Content")]
	public class PivotItem : DependencyObject
	{
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (string), typeof (PivotItem), new PropertyMetadata(default(string)));

		public string Header
		{
			get { return (string) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof(FrameworkElement), typeof(PivotItem), new PropertyMetadata(default(FrameworkElement)));

		public FrameworkElement Content
		{
			get { return (FrameworkElement)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
	}
}