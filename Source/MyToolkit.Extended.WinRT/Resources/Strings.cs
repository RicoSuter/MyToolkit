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

		public static string ButtonYes { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Ja" : "Yes"; } }
		public static string ButtonNo { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Nein" : "No"; } }
		public static string ButtonCancel { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Abbrechen" : "Cancel"; } }
		public static string ButtonOk { get { return "OK"; } }

		public static string CheckAllButton { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Alle wählen" : "Check all"; } }
		public static string UncheckAllButton { get { return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "de" ? "Alle abwählen" : "Uncheck all"; } }
	}
}
