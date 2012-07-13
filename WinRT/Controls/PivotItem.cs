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
		// TODO: change to FrameworkElement to remove DataTemplate usage in control usage
		//public static readonly DependencyProperty TemplateProperty =
		//	DependencyProperty.Register("Template", typeof(DataTemplate), typeof(PivotItem), new PropertyMetadata(default(DataTemplate)));

		//public DataTemplate Template
		//{
		//	get { return (DataTemplate)GetValue(TemplateProperty); }
		//	set { SetValue(TemplateProperty, value); }
		//}
	}
}