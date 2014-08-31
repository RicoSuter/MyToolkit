//-----------------------------------------------------------------------
// <copyright file="DataGridRow.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    /// <summary>A <see cref="DataGrid"/> row. </summary>
	public class DataGridRow : Grid
	{
        private ContentControl _detailsControl = null;
        private bool _isSelected = false;

        internal DataGridRow(DataGrid dataGrid, object item)
		{
			Item = item;
			DataGrid = dataGrid;
			
			var x = 0;
			var hasStar = false;
			foreach (var c in dataGrid.Columns)
			{
				var cell = c.GenerateElement(item);

                var content = new ContentPresenter();
                content.Content = cell.Control;
                //content.ContentTemplate = dataGrid.CellTemplate;
                content.Margin = new Thickness(10, 0, 0, 5); // TODO: Use template and remove margin
                content.Tag = cell; 
                
				SetColumn(content, x++);
				Children.Add(content);
			
				var def = c.CreateGridColumnDefinition();
				hasStar = hasStar || def.Width.IsStar;

				ColumnDefinitions.Add(def);
			}

			if (!hasStar)
				ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			// second row used for details view
			RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		}

        /// <summary>
        /// Gets the parent <see cref="DataGrid"/>. 
        /// </summary>
		public DataGrid DataGrid { get; private set; }

        /// <summary>
        /// Gets or sets the associated item. 
        /// </summary>
		public object Item { get; private set; }

        /// <summary>
        /// Gets a value indicathing whether the row is selected. 
        /// </summary>
		public bool IsSelected
		{
			get { return _isSelected; }
			internal set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					UpdateItemDetails();
				}
			}
		}

        /// <summary>
        /// Gets the list of <see cref="DataGridCell"/>. 
        /// </summary>
        public IList<DataGridCell> Cells
        {
            get
            {
                return Children
                    .OfType<ContentPresenter>()
                    .Where(c => c.Tag is DataGridCell)
                    .Select(c => (DataGridCell)c.Tag)
                    .ToList();
            }
        }

		internal void UpdateItemDetails()
		{
			if (DataGrid.ItemDetailsTemplate != null)
			{
				if (!_isSelected || !DataGrid.ShowItemDetails)
				{
					if (_detailsControl != null)
					{
						Children.Remove(_detailsControl);
						_detailsControl = null;
					}
				}
				else if (_isSelected && DataGrid.ShowItemDetails)
				{
					if (_detailsControl == null)
					{
						_detailsControl = new ContentControl();
						_detailsControl.Content = Item;
						_detailsControl.ContentTemplate = DataGrid.ItemDetailsTemplate;
						_detailsControl.VerticalContentAlignment = VerticalAlignment.Stretch;
						_detailsControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;

						Children.Add(_detailsControl);
						SetRow(_detailsControl, 1);
						SetColumnSpan(_detailsControl, ColumnDefinitions.Count);
					}
					else
						_detailsControl.ContentTemplate = DataGrid.ItemDetailsTemplate;
				}
			}

			foreach (var cell in Cells)
				cell.OnSelectedChanged(_isSelected);
		}
    }
}
