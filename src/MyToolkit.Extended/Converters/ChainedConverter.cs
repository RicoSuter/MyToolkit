//-----------------------------------------------------------------------
// <copyright file="ChainedConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace MyToolkit.Converters
{
    /// <summary>
    /// Used to compose multiple converters. 
    /// </summary>
    [ContentProperty(Name = "Converters")]
    public class ChainedConverter : DependencyObject, IValueConverter
    {
        public ChainedConverter()
        {
            Converters = new ObservableCollection<IValueConverter>();
        }

        public static readonly DependencyProperty ConvertersProperty =
            DependencyProperty.Register("Converters", typeof (ObservableCollection<IValueConverter>), typeof (ChainedConverter), new PropertyMetadata(default(ObservableCollection<IValueConverter>)));

        public ObservableCollection<IValueConverter> Converters
        {
            get { return (ObservableCollection<IValueConverter>) GetValue(ConvertersProperty); }
            set { SetValue(ConvertersProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            foreach (var c in Converters)
                value = c.Convert(value, targetType, parameter, language);
            return value; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            foreach (var c in Converters)
                value = c.ConvertBack(value, targetType, parameter, language);
            return value; 
        }
    }
}

#endif