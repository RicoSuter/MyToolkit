//-----------------------------------------------------------------------
// <copyright file="FrameworkElementExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WPF || SL5 || WP7 || WP8
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
#elif WINRT
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for <see cref="FrameworkElement"/> objects. </summary>
    public static class FrameworkElementExtensions
    {
        /// <summary>Gets the vertical scroll position of the element's <see cref="ScrollViewer"/>. </summary>
        /// <param name="element">The element which must have a <see cref="ScrollViewer"/> as child. </param>
        /// <returns>The scroll position. </returns>
        public static double GetVerticalScrollPosition(this FrameworkElement element)
        {
            var scrollViewer = element.FindVisualChild<ScrollViewer>();
            return scrollViewer.VerticalOffset;
        }

        /// <summary>Sets the horizontal scroll position of the element's <see cref="ScrollViewer"/>. </summary>
        /// <param name="element">The element which must have a <see cref="ScrollViewer"/> as child. </param>
        /// <returns>The scroll position. </returns>
        public static double GetHorizontalScrollPosition(this FrameworkElement element)
        {
            var scrollViewer = element.FindVisualChild<ScrollViewer>();
            return scrollViewer.HorizontalOffset;
        }

        /// <summary>Sets the horizontal scroll position of the element's <see cref="ScrollViewer"/>. </summary>
        /// <param name="element">The element which must have a <see cref="ScrollViewer"/> as child. </param>
        /// <param name="position">The scroll position. </param>
        public static void SetVerticalScrollPosition(this FrameworkElement element, double position)
        {
            var scrollViewer = element.FindVisualChild<ScrollViewer>();
            scrollViewer.ScrollToVerticalOffset(position);
        }

        /// <summary>Gets the horizontal scroll position of the element's <see cref="ScrollViewer"/>. </summary>
        /// <param name="element">The element which must have a <see cref="ScrollViewer"/> as child. </param>
        /// <param name="position">The scroll position. </param>
        public static void SetHorizontalScrollPosition(this FrameworkElement element, double position)
        {
            var scrollViewer = element.FindVisualChild<ScrollViewer>();
            scrollViewer.ScrollToHorizontalOffset(position);
        }

#if WPF || WINRT || WP8 || WP7 || SL5

        /// <summary>Checks whether an element which is contained in a container is currently visible on the screen. </summary>
        /// <param name="element">The element. </param>
        /// <param name="container">The element's container (e.g. a <see cref="ListBox"/>). </param>
        /// <returns>true if the element is visible to the user; false otherwise. </returns>
        public static bool IsVisibleOnScreen(this FrameworkElement element, FrameworkElement container)
        {
#if WPF
            if (!element.IsVisible)
                return false;

            var bounds = element.TransformToAncestor(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(bounds.TopLeft) || rect.Contains(bounds.BottomRight);
#else
            var bounds = element.TransformToVisual(container).TransformBounds(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            var rect = new Rect(0.0, 0.0, container.ActualWidth, container.ActualHeight);
            return rect.Contains(new Point(bounds.Left, bounds.Top)) || rect.Contains(new Point(bounds.Right, bounds.Right));
#endif
        }

#endif
    }
}