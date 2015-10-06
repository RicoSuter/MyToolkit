//-----------------------------------------------------------------------
// <copyright file="DateTimeConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

#if !WINRT
    using System.Windows.Data;
#else
    using Windows.UI.Xaml.Data;
    using Windows.Globalization.DateTimeFormatting;
#endif

namespace MyToolkit.Converters
{
    /// <summary>
    /// Converts a DateTime into its string representation. 
    /// </summary>
    public class DateTimeConverter : IValueConverter
    {
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";

            if (parameter == null)
                parameter = "datetime";

            switch (parameter.ToString().ToLower())
            {
                case "dayabbreviated":
                case "dayabb": return culture.DateTimeFormat.AbbreviatedDayNames[(int)((DateTime)value).DayOfWeek];
                case "day": return culture.DateTimeFormat.DayNames[(int)((DateTime)value).DayOfWeek];
                case "date": return ((DateTime)value).ToShortDateString();
                case "time": return ((DateTime)value).ToShortTimeString();
                case "timewithseconds": return ((DateTime)value).ToString("T");
                default: return ((DateTime)value).ToShortDateString() + " " + ((DateTime)value).ToShortTimeString();
            }
        }
#else
        public object Convert(object value, Type typeName, object parameter, string language)

        {
            if (value == null)
                return "";

            if (parameter == null)
                parameter = "datetime";

            switch (parameter.ToString().ToLower())
            {
                case "dayabbreviated":
                case "dayabb": return new DateTimeFormatter(YearFormat.None, MonthFormat.None, DayFormat.None, DayOfWeekFormat.Abbreviated).Format(((DateTime)value));
                case "day": return new DateTimeFormatter(YearFormat.None, MonthFormat.None, DayFormat.None, DayOfWeekFormat.Default).Format(((DateTime)value));
                case "date": return new DateTimeFormatter(YearFormat.Default, MonthFormat.Numeric, DayFormat.Default, DayOfWeekFormat.None).Format(((DateTime)value));
                case "time": return new DateTimeFormatter(HourFormat.Default, MinuteFormat.Default, SecondFormat.Default).Format(((DateTime)value));
                case "timewithoutseconds": return new DateTimeFormatter(HourFormat.Default, MinuteFormat.Default, SecondFormat.None).Format(((DateTime)value));
                default: return new DateTimeFormatter(YearFormat.Default, MonthFormat.Numeric, DayFormat.Default, DayOfWeekFormat.None).Format(((DateTime)value)) + " " +
                    new DateTimeFormatter(HourFormat.Default, MinuteFormat.Default, SecondFormat.Default).Format(((DateTime)value));
            }
        }
#endif

#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type typeName, object parameter, string language)
#endif
        {
            var text = value.ToString();
            if (String.IsNullOrEmpty(text))
                return null; 
            return DateTime.Parse(text);
        }
    }
}
