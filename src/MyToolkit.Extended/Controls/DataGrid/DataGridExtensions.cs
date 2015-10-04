//-----------------------------------------------------------------------
// <copyright file="DataGridExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    public static class DataGridExtensions
    {
        /// <summary>Gets or sets a value indicating whether to allow only a single selection which can be deselected.</summary>
        public static readonly DependencyProperty UseSingleSelectionAndDeselectionProperty =
            DependencyProperty.RegisterAttached("UseSingleSelectionAndDeselection", typeof(bool),
                typeof(DataGridExtensions), new PropertyMetadata(false, OnClickSelectionChanged));

        /// <summary>Gets a value indicating whether to allow only a single selection which can be deselected.</summary>
        /// <param name="obj">The <see cref="DependencyObject"/>. </param>
        /// <returns>The value. </returns>
        public static bool GetUseSingleSelectionAndDeselection(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseSingleSelectionAndDeselectionProperty);
        }

        /// <summary>Sets a value indicating whether to allow only a single selection which can be deselected.</summary>
        /// <param name="obj">The <see cref="DependencyObject"/>. </param>
        /// <param name="value">The value. </param>
        public static void SetUseSingleSelectionAndDeselection(DependencyObject obj, bool value)
        {
            obj.SetValue(UseSingleSelectionAndDeselectionProperty, value);
        }

        private static void OnClickSelectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = obj as DataGrid;
            if (dataGrid != null)
            {
                dataGrid.SelectionMode = SelectionMode.Multiple;

                if ((bool)e.NewValue)
                {
                    dataGrid.SelectionMode = SelectionMode.Multiple;
                    dataGrid.SelectionChanged += OnSelectionChanged;
                }
                else
                    dataGrid.SelectionChanged -= OnSelectionChanged;
            }
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var dataGrid = (DataGrid)sender;
                var valid = e.AddedItems[0];
                foreach (var item in dataGrid.SelectedItems.Where(i => i != valid).ToList())
                    dataGrid.SelectedItems.Remove(item);
            }
        }
    }
}

#endif