//-----------------------------------------------------------------------
// <copyright file="NavigationList.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
    public class NavigationList : ScrollableItemsControl
    {
        /// <summary>Occurs when the user wants to navigate to an item. </summary>
        public event EventHandler<NavigationListEventArgs> Navigate;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            ((UIElement)element).Tapped += OnTapped;
        }

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            OnNavigate(new NavigationListEventArgs(element.DataContext));
        }

        protected void OnNavigate(NavigationListEventArgs args)
        {
            var copy = Navigate;
            if (copy != null)
                copy(this, args);
        }
    }
}

#endif