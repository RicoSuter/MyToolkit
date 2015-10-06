//-----------------------------------------------------------------------
// <copyright file="ItemsControlExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif
using MyToolkit.UI;

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for <see cref="ItemsControl"/> objects. </summary>
    public static class ItemsControlExtensions
    {
        /// <summary>Gets all items of the <see cref="ItemsControl"/> which are currently visible on the screen. </summary>
        /// <typeparam name="T">The type of the items. </typeparam>
        /// <param name="itemsControl">The <see cref="ItemsControl"/>. </param>
        /// <returns>The visible items. </returns>
        public static List<T> GetVisibleItems<T>(this ItemsControl itemsControl)
        {
            var items = new List<T>();
            if (itemsControl.Items != null)
            {
                foreach (var item in itemsControl.Items)
                {
                    var itemContainer = (ListBoxItem)itemsControl.ItemContainerGenerator.ContainerFromItem(item);
                    if (itemContainer != null && itemContainer.IsVisibleOnScreen(itemsControl))
                        items.Add((T)item);
                    else if (items.Count > 0)
                        break;
                }
            }
            return items;
        }

        /// <summary>Gets the first item of the <see cref="ItemsControl"/> which are currently visible on the screen. </summary>
        /// <typeparam name="T">The type of the item. </typeparam>
        /// <param name="itemsControl">The <see cref="ItemsControl"/>. </param>
        /// <returns>The visible item. </returns>
        public static T GetFirstVisibleItem<T>(this ItemsControl itemsControl)
        {
            if (itemsControl.Items != null)
            {
                foreach (var item in itemsControl.Items)
                {
                    var itemContainer = (FrameworkElement)itemsControl.ItemContainerGenerator.ContainerFromItem(item);
                    if (itemContainer != null && itemContainer.IsVisibleOnScreen(itemsControl))
                        return (T)item;
                }
            }
            return default(T);
        }
    }
}