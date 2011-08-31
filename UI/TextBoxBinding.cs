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
	public static class TextBoxBinding
	{
		public static void ForceTextUpdate(object sender)
		{
			var textBox = sender as TextBox;
			if (textBox != null)
			{
				var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
				if (bindingExpression != null)
					bindingExpression.UpdateSource();
			} 
		}
	}
}
