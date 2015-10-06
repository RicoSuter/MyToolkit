//-----------------------------------------------------------------------
// <copyright file="NavigationGridView.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    public class NavigationGridView : GridView
    {
        public NavigationGridView()
        {
            SelectionChanged += OnSelectionChanged;
        }

        /// <summary>Occurs when the user wants to navigate to an item. </summary>
        public event EventHandler<NavigationListEventArgs> Navigate;

        protected void OnNavigate(NavigationListEventArgs args)
        {
            var copy = Navigate;
            if (copy != null)
                copy(this, args);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var item = SelectedItem;
            SelectedItem = null;
            if (item != null)
                OnNavigate(new NavigationListEventArgs(item));
        }
    }
}

#endif