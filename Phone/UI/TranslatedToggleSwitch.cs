using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace MyToolkit.UI
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
