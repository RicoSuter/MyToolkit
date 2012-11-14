using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public class DataGridRow : Grid
	{
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
                content.Margin = new Thickness(10, 0, 0, 5);
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

		public DataGrid DataGrid { get; private set; }
		public object Item { get; private set; }

		private ContentControl detailsControl = null;

		bool isSelected = false;
		public bool IsSelected
		{
			get { return isSelected; }
			set
			{
				if (isSelected != value)
				{
					isSelected = value;
					UpdateItemDetails();
				}
			}
		}

		public void UpdateItemDetails()
		{
			if (DataGrid.ItemDetailsTemplate != null)
			{
				if (!isSelected || !DataGrid.ShowItemDetails)
				{
					if (detailsControl != null)
					{
						Children.Remove(detailsControl);
						detailsControl = null;
					}
				}
				else if (isSelected && DataGrid.ShowItemDetails)
				{
					if (detailsControl == null)
					{
						detailsControl = new ContentControl();
						detailsControl.Content = Item;
						detailsControl.ContentTemplate = DataGrid.ItemDetailsTemplate;
						detailsControl.VerticalContentAlignment = VerticalAlignment.Stretch;
						detailsControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;

						Children.Add(detailsControl);
						SetRow(detailsControl, 1);
						SetColumnSpan(detailsControl, ColumnDefinitions.Count);
					}
					else
						detailsControl.ContentTemplate = DataGrid.ItemDetailsTemplate;
				}
			}

			foreach (var cell in Cells)
				cell.OnSelectedChanged(isSelected);
		}

		public IList<DataGridCell> Cells
		{
			get 
			{ 
				return Children.OfType<ContentPresenter>().
					Where(c => c.Tag is DataGridCell).
					Select(c => (DataGridCell)c.Tag).ToList(); 
			}
		}
	}
}
