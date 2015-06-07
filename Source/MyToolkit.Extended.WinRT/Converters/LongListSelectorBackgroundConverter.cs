using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Converters
{
    public class LongListSelectorBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var group = (ICollection)value;
            var count = group.Count;

            return count == 0
                ? Application.Current.Resources["PhoneChromeBrush"]
                : Application.Current.Resources["PhoneAccentBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
