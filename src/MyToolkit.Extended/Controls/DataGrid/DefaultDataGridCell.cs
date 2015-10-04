#if WINRT

using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
    public class DefaultDataGridCell : DataGridCellBase
    {
        public DefaultDataGridCell(FrameworkElement control)
            : base(control) { }

        /// <summary>Called when the cell's row gets selected or unselected.</summary>
        /// <param name="isSelected">Indicates whether the cell is selected or not.</param>
        public override void OnSelectedChanged(bool isSelected)
        {
        }
    }
}

#endif