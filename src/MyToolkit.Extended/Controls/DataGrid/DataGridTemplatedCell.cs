#if WINRT

using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
    public class DataGridTemplatedCell : DataGridCellBase
    {
        public DataGridTemplatedCell(FrameworkElement control, bool onlyVisibleOnSelection)
            : base(control)
        {
            OnlyVisibleOnSelection = onlyVisibleOnSelection;
        }

        /// <summary>Gets or sets a value indicating whether the cell is only visible when the row is selected.</summary>
        public bool OnlyVisibleOnSelection { get; set; }

        /// <summary>Called when the cell's row gets selected or unselected. </summary>
        /// <param name="isSelected">Indicates whether the cell is selected or not. </param>
        public override void OnSelectedChanged(bool isSelected)
        {
            if (OnlyVisibleOnSelection)
                Control.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

#endif