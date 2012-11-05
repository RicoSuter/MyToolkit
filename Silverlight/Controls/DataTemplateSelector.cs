using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.Controls
{
	public abstract class DataTemplateSelector : ContentControl
	{
		public virtual DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			return null;
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			var tpl = SelectTemplate(newContent, this);
			ContentTemplate = tpl; 
		}
	}
}