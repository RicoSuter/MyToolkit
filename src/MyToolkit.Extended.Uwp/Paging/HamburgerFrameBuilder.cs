//-----------------------------------------------------------------------
// <copyright file="HamburgerFrameBuilder.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using Windows.UI.Xaml;
using MyToolkit.Controls;

namespace MyToolkit.Paging
{
    /// <summary>A factory to create a <see cref="Hamburger"/> which works in conjunction with a <see cref="MtFrame"/>.</summary>
    public class HamburgerFrameBuilder
    {
        /// <summary>Initializes a new instance of the <see cref="HamburgerFrameBuilder"/> class.</summary>
        public HamburgerFrameBuilder()
        {
            DeselectWhenPageNotFound = true;

            Frame = new MtFrame();
            Frame.PageAnimation = null;
            Frame.Navigated += OnFrameNavigated;

            Hamburger = new Hamburger();
            Hamburger.Content = Frame;
            Hamburger.ItemChanged += OnHamburgerItemChanged;
            Hamburger.PaneVisibilityChanged += HamburgerOnPaneVisibilityChanged;
        }

        /// <summary>Gets or sets a value indicating whether to deselect the current item when the page could not be found.</summary>
        public bool DeselectWhenPageNotFound { get; set; }

        /// <summary>Gets or sets a value indicating whether to manage the margins of the page's AppBars to avoid overlapping AppBars.</summary>
        public bool ManagePageAppBarMargins { get; set; } = true;

        /// <summary>Gets the frame.</summary>
        public MtFrame Frame { get; private set; }

        /// <summary>Gets or sets the hamburger control.</summary>
        public Hamburger Hamburger { get; set; }

        private void OnFrameNavigated(object sender, MtNavigationEventArgs args)
        {
            var currentItem = Hamburger.TopItems
                .Concat(Hamburger.BottomItems)
                .OfType<PageHamburgerItem>()
                .FirstOrDefault(i => i.PageType == Frame.CurrentPage.Type);

            if (DeselectWhenPageNotFound || currentItem != null)
                Hamburger.SelectedItem = currentItem;

            UpdateCurrentPageAppBarMargin();
        }

        private async void OnHamburgerItemChanged(object sender, HamburgerItemChangedEventArgs args)
        {
            if (args.Item is PageHamburgerItem)
            {
                var pageItem = (PageHamburgerItem)args.Item;
                if (pageItem.PageType != null)
                {
                    if (pageItem.UseSinglePageInstance)
                        await Frame.NavigateToExistingOrNewPageAsync(pageItem.PageType, pageItem.PageParameter);
                    else
                        await Frame.NavigateAsync(pageItem.PageType, pageItem.PageParameter);
                }
            }
        }

        private void HamburgerOnPaneVisibilityChanged(object sender, bool b)
        {
            UpdateCurrentPageAppBarMargin();
        }

        private void UpdateCurrentPageAppBarMargin()
        {
            if (ManagePageAppBarMargins)
            {
                if (Frame?.CurrentPage?.Page?.TopAppBar != null)
                    Frame.CurrentPage.Page.TopAppBar.Margin = new Thickness(Hamburger.IsPaneVisible ? 48 : 0, 0, 0, 0);

                if (Frame?.CurrentPage?.Page?.BottomAppBar != null)
                    Frame.CurrentPage.Page.BottomAppBar.Margin = new Thickness(Hamburger.IsPaneVisible ? 48 : 0, 0, 0, 0);
            }
        }
    }
}