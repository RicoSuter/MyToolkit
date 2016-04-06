//-----------------------------------------------------------------------
// <copyright file="DataGridColumnEventArgs.cs" company="MyToolkit">
//     Copyright (c) Bram Stoeller. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Bram Stoeller, bram@stoeller.nl</author>
//-----------------------------------------------------------------------

#if WINRT

using MyToolkit.Controls;
using System;

namespace MyToolkit.Controls
{
    public class DataGridColumnEventArgs : EventArgs
    {
        internal DataGridColumnEventArgs(DataGridColumnBase column)
        {
            Column = column;
        }

        /// <summary>Gets ordering column. </summary>
        public DataGridColumnBase Column { private set; get; }

    }
}

#endif