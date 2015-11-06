//-----------------------------------------------------------------------
// <copyright file="PageHamburgerItem.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Controls
{
    /// <summary>A hamburger icon which navigates to a page.</summary>
    public class PageHamburgerItem : HamburgerItem
    {
        /// <summary>Initializes a new instance of the <see cref="PageHamburgerItem"/> class.</summary>
        public PageHamburgerItem()
        {
            UseSinglePageInstance = true;
            AutoClosePane = true; 
        }

        /// <summary>Gets or sets the type of the page.</summary>
        public Type PageType { get; set; }

        /// <summary>Gets or sets the page navigation parameter.</summary>
        public object PageParameter { get; set; }

        /// <summary>Gets or sets a value indicating whether to use a single page instance of the given type (default: true).</summary>
        public bool UseSinglePageInstance { get; set; }
    }
}