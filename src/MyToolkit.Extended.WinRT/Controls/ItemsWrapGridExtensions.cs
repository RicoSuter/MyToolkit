//-----------------------------------------------------------------------
// <copyright file="ItemsWrapGridExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    /// <summary>Attached property extensions for the <see cref="ItemsWrapGrid"/>.</summary>
    public class ItemsWrapGridExtensions
    {
        public static readonly DependencyProperty ItemsMinWidthProperty = DependencyProperty.RegisterAttached(
            "ItemsMinWidth", typeof(double), typeof(ItemsWrapGridExtensions), new PropertyMetadata(default(double), OnValueChanged));

        public static void SetItemsMinWidth(DependencyObject element, double value)
        {
            element.SetValue(ItemsMinWidthProperty, value);
        }

        public static double GetItemsMinWidth(DependencyObject element)
        {
            return (double)element.GetValue(ItemsMinWidthProperty);
        }

        public static readonly DependencyProperty ItemsMinHeight = DependencyProperty.RegisterAttached(
            "ItemsMaxWidth", typeof(double), typeof(ItemsWrapGridExtensions), new PropertyMetadata(default(double)));

        public static void SetItemsMinHeight(DependencyObject element, double value)
        {
            element.SetValue(ItemsMinHeight, value);
        }

        public static double GetItemsMinHeight(DependencyObject element)
        {
            return (double)element.GetValue(ItemsMinHeight);
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var element = obj as FrameworkElement;
            if (element != null)
            {
                var eventRegistration = new SizeChangedRegistration(element);
                element.Loaded += delegate
                {
                    UpdateElement(element);
                    element.SizeChanged += eventRegistration.OnSizeChanged;
                };

                element.Unloaded += delegate
                {
                    element.SizeChanged -= eventRegistration.OnSizeChanged;
                };

                UpdateElement(element);
            }
        }

        private static void UpdateElement(FrameworkElement element)
        {
            UpdateElementWidth(element);
            UpdateElementHeight(element);
        }

        private static void UpdateElementWidth(FrameworkElement element)
        {
            var minimumWidth = GetItemsMinWidth(element);
            if (minimumWidth > 0)
            {
                var width = element.ActualWidth;
                var columnCount = (int)(width / minimumWidth);
                var difference = width - columnCount * minimumWidth;

                if (width > minimumWidth)
                    ((ItemsWrapGrid)element).ItemWidth = minimumWidth + difference / columnCount;
            }
        }

        private static void UpdateElementHeight(FrameworkElement element)
        {
            var minimumHeight = GetItemsMinHeight(element);
            if (minimumHeight > 0)
            {
                var height = element.ActualHeight;
                var columnCount = (int)(height / minimumHeight);
                var difference = height - columnCount * minimumHeight;

                if (height > minimumHeight)
                    ((ItemsWrapGrid)element).ItemHeight = minimumHeight + difference / columnCount;
            }
        }

        private class SizeChangedRegistration
        {
            private readonly FrameworkElement _element;

            public SizeChangedRegistration(FrameworkElement element)
            {
                _element = element;
            }

            public void OnSizeChanged(object sender, SizeChangedEventArgs e)
            {
                UpdateElement(_element);
            }
        }
    }
}
