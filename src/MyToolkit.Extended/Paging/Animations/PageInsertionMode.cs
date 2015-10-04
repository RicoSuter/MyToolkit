//-----------------------------------------------------------------------
// <copyright file="PageInsertionMode.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

namespace MyToolkit.Paging.Animations
{
    /// <summary>Enumeration of the possible page insertion modes. </summary>
    public enum PageInsertionMode
    {
        /// <summary>Inserts the next page over the previous page before starting the animations so that both pages are in the visual tree during the animations. </summary>
        ConcurrentAbove,

        /// <summary>Inserts the next page below the previous page before starting the animations so that both pages are in the visual tree during the animations. </summary>
        ConcurrentBelow,

        /// <summary>Inserts the next page after the navigating from animation and removes the previous page. </summary>
        Sequential
    }
}

#endif