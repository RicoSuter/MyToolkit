#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace MyToolkit.Controls
{
	public abstract class DataTemplateSelector : ContentControl
	{
		public abstract DataTemplate SelectTemplate(object item, DependencyObject container);

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			var tpl = SelectTemplate(newContent, this);
			ContentTemplate = tpl; 
		}
	}
}