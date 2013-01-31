using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
	public class DataGridTemplatedCell : DataGridCell
	{
		public bool OnlyVisibleOnSelection { get; set; }

		public DataGridTemplatedCell(FrameworkElement control, bool onlyVisibleOnSelection)
			: base(control) 
		{
			OnlyVisibleOnSelection = onlyVisibleOnSelection; 
		}

		public override void OnSelectedChanged(bool isSelected)
		{
			if (OnlyVisibleOnSelection)
				Control.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed; 
		}
	}
}