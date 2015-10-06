//-----------------------------------------------------------------------
// <copyright file="TextBinding.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#else
using System;
using System.Windows;
using System.Windows.Controls;
#endif

namespace MyToolkit.UI
{
    /// <summary>
    /// Supports a PropertyChanged-Trigger for DataBindings
    /// Works for only TextBoxes and PasswordBoxes
    ///  <TextBox 
    ///     Text="{Binding FirstName, Mode=TwoWay}"
    ///     local:TextBinding.UpdateSourceOnChange="True" />
    /// 
    /// Code is based by Thomas Huber
    /// http://www.thomasclaudiushuber.com/blog/2009/07/17/here-it-is-the-updatesourcetrigger-for-propertychanged-in-silverlight/
    /// 
    /// Idea for allowing PasswordBox as well is from Dele Olowoyo
    /// http://www.pragmaticpattern.wordpress.com
    /// http://www.dcubeapps.com
    /// </summary>
    public class TextBinding
    {
        public static void ForceTextUpdate(object sender)
        {
            var type = sender.GetType();
            if (type == typeof (TextBox))
            {
                var textBox = sender as TextBox;
                if (textBox != null)
                {
                    var bindingExpression = textBox.GetBindingExpression(TextBox.TextProperty);
                    if (bindingExpression != null)
                        bindingExpression.UpdateSource();
                }
            }
            else
            {
#if !WPF
                var textBox = sender as PasswordBox;
                if (textBox != null)
                {
                    var bindingExpression = textBox.GetBindingExpression(PasswordBox.PasswordProperty);
                    if (bindingExpression != null)
                        bindingExpression.UpdateSource();
                }
#else
                throw new NotSupportedException("ForceTextUpdate is not supported for PasswordBox in WPF.");
#endif
            }
        }

        public static bool GetUpdateSourceOnChange(DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdateSourceOnChangeProperty);
        }

        public static void SetUpdateSourceOnChange(DependencyObject obj, bool value)
        {
            obj.SetValue(UpdateSourceOnChangeProperty, value);
        }

        public static readonly DependencyProperty UpdateSourceOnChangeProperty =
            DependencyProperty.RegisterAttached( "UpdateSourceOnChange", typeof(bool), typeof(TextBinding),
                new PropertyMetadata(false, OnUpdateSourceOnChangePropertyChanged));

        private static void OnUpdateSourceOnChangePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == e.OldValue)
                return;
            HandleUpdateSourceOnChangeEventAppend(obj, (bool)e.NewValue);
        }

        private static void HandleUpdateSourceOnChangeEventAppend(object sender, bool value)
        {
            var type = sender.GetType();
            if (type == typeof(TextBox))
                HandleUpdateSourceOnChangeEventAppendTextBox(sender, value);
            else if (type == typeof(PasswordBox))
                HandleUpdateSourceOnChangeEventAppendPasswordBox(sender, value);
        }

        private static void HandleUpdateSourceOnChangeEventAppendTextBox(object sender, bool value)
        {
            var item = sender as TextBox;
            if (item == null)
                return;

            if (value)
                item.TextChanged += UpdateSourceOnChangePropertyChanged;
            else
                item.TextChanged -= UpdateSourceOnChangePropertyChanged;
        }

        private static void HandleUpdateSourceOnChangeEventAppendPasswordBox(object sender, bool value)
        {
            var item = sender as PasswordBox;
            if (item == null)
                return;

            if (value)
                item.PasswordChanged += UpdateSourceOnChangePropertyChanged;
            else
                item.PasswordChanged -= UpdateSourceOnChangePropertyChanged;
        }

        private static void UpdateSourceOnChangePropertyChanged(object sender, RoutedEventArgs e)
        {
            var dp = GetDependancyPropertyForText(sender);
            if (dp == null)
                return;

            var bind = ((FrameworkElement)sender).GetBindingExpression(dp);
            if (bind != null)
                bind.UpdateSource();
        }

        private static DependencyProperty GetDependancyPropertyForText(object sender)
        {
            var type = sender.GetType();
            DependencyProperty returnVal = null;

            if (type == typeof(TextBox))
                returnVal = TextBox.TextProperty;
#if !WPF
            else if (type == typeof(PasswordBox))
                returnVal = PasswordBox.PasswordProperty;
#endif
            return returnVal;
        }
    }
}
