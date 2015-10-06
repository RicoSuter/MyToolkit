//-----------------------------------------------------------------------
// <copyright file="DependencyObjectExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WPF || SL5 || WP7 || WP8
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for <see cref="DependencyObject"/> objects. </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>Traverses the visual tree and returns the first child of the desired type. </summary>
        /// <typeparam name="T">The child type to find. </typeparam>
        /// <param name="obj">The parent object. </param>
        /// <returns>The child object. </returns>
        public static T FindVisualChild<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return (T)child;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        /// <summary>Traverses the visual tree and returns all children of the desired type. </summary>
        /// <typeparam name="T">The child type to find. </typeparam>
        /// <param name="obj">The parent object. </param>
        /// <returns>The children. </returns>
        public static List<T> FindVisualChildren<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            var results = new List<T>();
            FindVisualChildren<T>(obj, results);
            return results;
        }

        private static void FindVisualChildren<T>(DependencyObject obj, List<T> results)
            where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    results.Add((T)child);

                FindVisualChildren(child, results);
            }
        }
    }
}
