using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToolkit.Resources
{
	public static class Strings
	{
		public static string Minute { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Minute" : "minute"; } }
		public static string Minutes { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Minuten" : "minutes"; } }
		public static string Hours { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Stunden" : "hours"; } }
		public static string Hour { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Stunde" : "hour"; } }
		public static string Day { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Tag" : "day"; } }
		public static string Days { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Tage" : "days"; } }
	}
}
