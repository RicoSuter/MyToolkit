//-----------------------------------------------------------------------
// <copyright file="DataGridColumnBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    public abstract class DataGridColumnBase : DependencyObject
    {
        public static readonly DependencyProperty CanSortProperty =
            DependencyProperty.Register("CanSort", typeof(bool), typeof(DataGridColumnBase), new PropertyMetadata(true));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataGridColumnBase), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsAscendingProperty =
            DependencyProperty.Register("IsAscending", typeof(bool), typeof(DataGridColumnBase), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsAscendingDefaultProperty =
            DependencyProperty.Register("IsAscendingDefault", typeof(bool), typeof(DataGridColumnBase), new PropertyMetadata(true));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(DataGridColumnBase), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(GridLength), typeof(DataGridColumnBase), new PropertyMetadata(default(GridLength)));

        /// <summary>Gets or sets a value indicating whether the column can be sorted. </summary>
        public bool CanSort
        {
            get { return (bool)GetValue(CanSortProperty); }
            set { SetValue(CanSortProperty, value); }
        }

        /// <summary>Gets a value indicating whether the the column is selected and used for sorting. 
        /// This property should not be set directly, use the SelectColumn method on <see cref="DataGrid"/>. </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            internal set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>Gets a value indicating whether the column is sorted ascending (otherwise descending). 
        /// This property should not be set directly, use the SelectColumn method on <see cref="DataGrid"/>. </summary>
        public bool IsAscending
        {
            get { return (bool)GetValue(IsAscendingProperty); }
            internal set { SetValue(IsAscendingProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether ascending sorting is default 
        /// (first click on the column will sort it ascending, otherwise descending). </summary>
        public bool IsAscendingDefault
        {
            get { return (bool)GetValue(IsAscendingDefaultProperty); }
            set { SetValue(IsAscendingDefaultProperty, value); }
        }

        /// <summary>Gets or sets the header. </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>Gets or sets the width of the column. </summary>
        public GridLength Width
        {
            get { return (GridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>Generates the cell for the given item. </summary>
        /// <param name="dataItem">The item to generate the cell for. </param>
        /// <returns>The <see cref="DataGridCellBase"/>. </returns>
        public abstract DataGridCellBase GenerateElement(object dataItem);

        /// <summary>Gets the property path which is used for sorting. </summary>
        public abstract PropertyPath OrderPropertyPath { get; }

        /// <summary>Creates a new column definition </summary>
        /// <returns>The column definition. </returns>
        internal ColumnDefinition CreateGridColumnDefinition()
        {
            return new ColumnDefinition { Width = Width };
        }
    }
}
