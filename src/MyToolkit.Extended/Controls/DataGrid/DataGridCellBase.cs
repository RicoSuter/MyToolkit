//-----------------------------------------------------------------------
// <copyright file="DataGridCellBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
    public abstract class DataGridCellBase
    {
        /// <summary>Initializes a new instance of the <see cref="DataGridCellBase"/> class. </summary>
        /// <param name="control">The control. </param>
        protected DataGridCellBase(FrameworkElement control)
        {
            Control = control;
        }

        /// <summary>Gets the UI control of the cell. </summary>
        public FrameworkElement Control { get; private set; }

        /// <summary>Called when the cell's row gets selected or unselected. </summary>
        /// <param name="isSelected">Indicates whether the cell is selected or not. </param>
        public abstract void OnSelectedChanged(bool isSelected);
    }
}

#endif