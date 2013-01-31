using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Controls
{
	public class DataGridTemplatedColumn : DataGridColumn
	{
		private DataTemplate cellTemplate;
		public DataTemplate CellTemplate
		{
			get { return cellTemplate; }
			set { cellTemplate = value; }
		}

		private Binding order;
		public Binding Order
		{
			get { return order; }
			set { order = value; }
		}

		public override PropertyPath OrderPropertyPath
		{
			get { return order.Path; }
		}

		public bool OnlyVisibleOnSelection { get; set; }

		public override DataGridCell GenerateElement(object dataItem)
		{
			var control = new ContentControl();
			control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			control.VerticalContentAlignment = VerticalAlignment.Stretch;
			control.Content = dataItem;
			control.ContentTemplate = cellTemplate;

			if (OnlyVisibleOnSelection)
				control.Visibility = IsSelected ? Visibility.Visible : Visibility.Collapsed; 
			return new DataGridTemplatedCell(control, OnlyVisibleOnSelection);
		}
	}
}