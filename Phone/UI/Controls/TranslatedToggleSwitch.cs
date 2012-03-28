using System.Globalization;
using System.Windows;
using Microsoft.Phone.Controls;

namespace MyToolkit.UI.Controls
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
			var text = isChecked ? "On" : "Off";
				
			switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
			{
				case "de": text = isChecked ? "Ein" : "Aus"; break;
				case "fr": text = isChecked ? "Activé" : "Désactivé"; break;
			}

			Content = text; 
		}
	}
}
