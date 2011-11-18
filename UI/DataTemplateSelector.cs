using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyToolkit.UI
{
	public abstract class DataTemplateSelector : ContentControl
	{
		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);
			ContentTemplate = SelectTemplate(newContent, this);
		}

		public abstract DataTemplate SelectTemplate(object item, DependencyObject container);
	}
}
