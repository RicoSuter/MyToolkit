//-----------------------------------------------------------------------
// <copyright file="MtItemsControl.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT


using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    /// <summary>A <see cref="ItemsControl"/> with additional features. </summary>
    public class MtItemsControl : ItemsControl
    {
        /// <summary>Occurs when a container for an item gets prepared. </summary>
        public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var copy = PrepareContainerForItem;
            if (copy != null)
                copy(this, new PrepareContainerForItemEventArgs(element, item));
        }
    }

    /// <summary>A <see cref="ItemsControl"/> with additional features. </summary>
    [Obsolete("Use MtItemsControl instead. 8/31/2014")]
    public class ExtendedItemsControl : MtItemsControl
    {
    }
}

#endif