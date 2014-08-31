using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
    public abstract class DataGridCell
	{
	    protected DataGridCell(FrameworkElement control)
        {
            Control = control;
        }

        /// <summary>Gets the cell's UI control. </summary>
		public FrameworkElement Control { get; private set; }

        /// <summary>Called when the cell's row gets selected or unselected. </summary>
        /// <param name="isSelected">Indicates whether the cell is selected or not. </param>
		public abstract void OnSelectedChanged(bool isSelected);
	}
}
