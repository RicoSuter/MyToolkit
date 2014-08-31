//-----------------------------------------------------------------------
// <copyright file="DataGridTemplatedColumn.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Controls
{
	public class DataGridTemplatedColumn : DataGridColumn
	{
		private DataTemplate _cellTemplate;
        private Binding _order;
        
        /// <summary>Gets or sets the cell data template. </summary>
        public DataTemplate CellTemplate
		{
			get { return _cellTemplate; }
			set { _cellTemplate = value; }
		}

        /// <summary>Gets or sets the binding which is used for sorting. </summary>
		public Binding Order
		{
			get { return _order; }
			set { _order = value; }
		}

		public override PropertyPath OrderPropertyPath
		{
			get { return _order.Path; }
		}

        /// <summary>Gets or sets a value indicating whether the column is only visible when the column is selected.  </summary>
        public bool OnlyVisibleOnSelection { get; set; }

		public override DataGridCell GenerateElement(object dataItem)
		{
			var control = new ContentControl();
			control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			control.VerticalContentAlignment = VerticalAlignment.Stretch;
			control.Content = dataItem;
			control.ContentTemplate = _cellTemplate;

			if (OnlyVisibleOnSelection)
				control.Visibility = IsSelected ? Visibility.Visible : Visibility.Collapsed; 

			return new DataGridTemplatedCell(control, OnlyVisibleOnSelection);
		}
	}
}