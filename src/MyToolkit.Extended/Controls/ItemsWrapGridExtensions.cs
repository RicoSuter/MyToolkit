//-----------------------------------------------------------------------
// <copyright file="ItemsWrapGridExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

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
            "ItemsMaxWidth", typeof(double), typeof(ItemsWrapGridExtensions), new PropertyMetadata(default(double), OnValueChanged));

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
            var grid = (ItemsWrapGrid)obj;
            if (grid != null)
            {
                var eventRegistration = new GridSizeChangedRegistration(grid);

                grid.Loaded -= eventRegistration.OnLoaded;
                grid.Loaded += eventRegistration.OnLoaded;

                grid.Unloaded -= eventRegistration.OnUnloaded;
                grid.Unloaded += eventRegistration.OnUnloaded;

                UpdateElement(grid);
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
                var maximumRowsOrColumns = ((ItemsWrapGrid)element).MaximumRowsOrColumns;

                var width = element.ActualWidth;
                var columnCount = (int)(width / minimumWidth);
                var difference = width - columnCount * minimumWidth;

                if (width > minimumWidth)
                {
                    if (maximumRowsOrColumns > 0 && columnCount > maximumRowsOrColumns)
                        ((ItemsWrapGrid)element).ItemWidth = minimumWidth + minimumWidth / maximumRowsOrColumns;
                    else
                        ((ItemsWrapGrid)element).ItemWidth = minimumWidth + difference / columnCount;
                }
            }
        }

        private static void UpdateElementHeight(FrameworkElement element)
        {
            var minimumHeight = GetItemsMinHeight(element);
            if (minimumHeight > 0)
            {
                var maximumRowsOrColumns = ((ItemsWrapGrid) element).MaximumRowsOrColumns;

                var height = element.ActualHeight;
                var rowCount = (int)(height / minimumHeight);
                var difference = height - rowCount * minimumHeight;

                if (height > minimumHeight)
                {
                    if (maximumRowsOrColumns > 0 && rowCount > maximumRowsOrColumns)
                        ((ItemsWrapGrid)element).ItemHeight = minimumHeight + minimumHeight / maximumRowsOrColumns;
                    else
                        ((ItemsWrapGrid)element).ItemHeight = minimumHeight + difference / rowCount;
                }
            }
        }

        private class GridSizeChangedRegistration
        {
            private readonly FrameworkElement _element;

            public GridSizeChangedRegistration(FrameworkElement element)
            {
                _element = element;
            }

            public void OnLoaded(object sender, RoutedEventArgs e)
            {
                _element.SizeChanged += OnSizeChanged;
            }

            public void OnUnloaded(object sender, RoutedEventArgs e)
            {
                _element.SizeChanged -= OnSizeChanged;
            }

            public void OnSizeChanged(object sender, SizeChangedEventArgs e)
            {
                UpdateElement(_element);
            }
        }
    }
}

#endif