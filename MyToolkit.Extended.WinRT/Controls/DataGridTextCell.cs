using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
	public class DefaultDataGridCell : DataGridCell
	{
		public DefaultDataGridCell(FrameworkElement control)
			: base(control) { }

        /// <summary>
        /// Calles when the cell's row gets selected or unselected. 
        /// </summary>
        /// <param name="isSelected">Indicates whether the cell is selected or not. </param>
		public override void OnSelectedChanged(bool isSelected)
		{
			
		}
	}

	public abstract class DataGridCell
	{
	    protected DataGridCell(FrameworkElement control)
        {
            Control = control;
        }

        /// <summary>
        /// Gets the cell's UI control. 
        /// </summary>
		public FrameworkElement Control { get; private set; }

        /// <summary>
        /// Calles when the cell's row gets selected or unselected. 
        /// </summary>
        /// <param name="isSelected">Indicates whether the cell is selected or not. </param>
		public abstract void OnSelectedChanged(bool isSelected);
	}
}
