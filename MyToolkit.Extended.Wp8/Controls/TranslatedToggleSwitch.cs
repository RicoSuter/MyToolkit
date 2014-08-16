using System.Globalization;
using System.Windows;
using Microsoft.Phone.Controls;
using MyToolkit.Resources;

namespace MyToolkit.Controls
{
	public class TranslatedToggleSwitch : ToggleSwitch
	{
		public TranslatedToggleSwitch()
		{
			UpdateText(null, null);
			Checked += UpdateText;
			Unchecked += UpdateText;
		}

		private void UpdateText(object sender, RoutedEventArgs routedEventArgs)
		{
			var isChecked = IsChecked.HasValue && IsChecked.Value;
			var text = isChecked ? Strings.On : Strings.Off;
			Content = text; 
		}
	}
}
