//-----------------------------------------------------------------------
// <copyright file="HamburgerFrameBuilder.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
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
            Hamburger.ItemChanged += async (s, e) => await OnSelectedHamburgerItemChanged(s, e);
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


        /// <summary>Called when the selected hamburger item has changed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="HamburgerItemChangedEventArgs"/> instance containing the event data.</param>
        /// <returns>The task.</returns>
        protected virtual async Task OnSelectedHamburgerItemChanged(object sender, HamburgerItemChangedEventArgs args)
        {
            if (args.Item is PageHamburgerItem)
            {
                var pageItem = (PageHamburgerItem)args.Item;
                if (pageItem.PageType != null)
                {
                    if (pageItem.UseSinglePageInstance)
                    {
                        var page = Frame.Pages.Reverse().FirstOrDefault(p => IsHamburgerItemForPage(pageItem, p));
                        if (page != null)
                            await Frame.CopyToTopAndNavigateAsync(page);
                        else
                            await Frame.NavigateAsync(pageItem.PageType, pageItem.PageParameter);
                    }
                    else
                        await Frame.NavigateAsync(pageItem.PageType, pageItem.PageParameter);
                }
            }
        }

        /// <summary>Determines whether the HamburgerItem represents the given page description.</summary>
        /// <param name="item">The hamburger item.</param>
        /// <param name="pageDescription">The page description.</param>
        /// <returns>true or false.</returns>
        protected virtual bool IsHamburgerItemForPage(HamburgerItem item, MtPageDescription pageDescription)
        {
            return item is PageHamburgerItem && ((PageHamburgerItem)item).PageType == pageDescription.Type;
        }

        private void OnFrameNavigated(object sender, MtNavigationEventArgs args)
        {
            var currentItem = Hamburger.TopItems
                .Concat(Hamburger.BottomItems)
                .FirstOrDefault(i => IsHamburgerItemForPage(i, Frame.CurrentPage));

            if (DeselectWhenPageNotFound || currentItem != null)
                Hamburger.SelectedItem = currentItem;

            UpdateCurrentPageAppBarMargin();
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