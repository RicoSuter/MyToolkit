//-----------------------------------------------------------------------
// <copyright file="DataGridTemplatedColumn.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Controls
{
    public class DataGridTemplatedColumn : DataGridColumnBase
    {
        /// <summary>Gets or sets the cell data template. </summary>
        public DataTemplate CellTemplate { get; set; }

        /// <summary>Gets or sets the binding which is used for sorting. </summary>
        public Binding Order { get; set; }

        /// <summary>Gets the property path which is used for sorting. </summary>
        public override PropertyPath OrderPropertyPath
        {
            get { return Order.Path; }
        }

        /// <summary>Gets or sets a value indicating whether the column is only visible when the column is selected.  </summary>
        public bool OnlyVisibleOnSelection { get; set; }

        /// <summary>Generates the cell for the given item.</summary>
        /// <param name="dataItem">The item to generate the cell for.</param>
        /// <returns>The <see cref="DataGridCellBase" />.</returns>
        public override DataGridCellBase CreateCell(object dataItem)
        {
            var control = new ContentControl();
            control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            control.VerticalContentAlignment = VerticalAlignment.Stretch;
            control.Content = dataItem;
            control.ContentTemplate = CellTemplate;

            if (OnlyVisibleOnSelection)
                control.Visibility = IsSelected ? Visibility.Visible : Visibility.Collapsed;

            return new DataGridTemplatedCell(control, OnlyVisibleOnSelection);
        }
    }
}

#endif