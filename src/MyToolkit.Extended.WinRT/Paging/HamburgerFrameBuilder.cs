//-----------------------------------------------------------------------
// <copyright file="HamburgerFrameBuilder.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
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
            Frame.Navigated += FrameOnNavigated;

            Hamburger = new Hamburger();
            Hamburger.Content = Frame; 
            Hamburger.ItemChanged += HamburgerOnItemChanged;
        }

        /// <summary>Gets or sets a value indicating whether to deselect the current item when the page could not be found.</summary>
        public bool DeselectWhenPageNotFound { get; set; }

        /// <summary>Gets the frame.</summary>
        public MtFrame Frame { get; private set; }

        /// <summary>Gets or sets the hamburger control.</summary>
        public Hamburger Hamburger { get; set; }

        public async Task<bool> MoveOrNavigateToPageAsync(Type pageType, object pageParamter = null)
        {
            var existingPage = Frame.GetNearestPageOfTypeInBackStack(pageType);
            if (existingPage != null)
            {
                existingPage.Parameter = pageParamter;
                return await Frame.MoveToTopAndNavigateAsync(existingPage);
            }
            return await Frame.NavigateAsync(pageType, pageParamter);
        }

        private void FrameOnNavigated(object sender, MtNavigationEventArgs args)
        {
            var currentItem = Hamburger.TopItems
                .Concat(Hamburger.BottomItems)
                .OfType<PageHamburgerItem>()
                .FirstOrDefault(i => i.PageType == Frame.CurrentPage.Type);

            if (DeselectWhenPageNotFound || currentItem != null)
                Hamburger.SelectedItem = currentItem;
        }

        private async void HamburgerOnItemChanged(object sender, HamburgerItemChangedEventArgs args)
        {
            if (args.Item is PageHamburgerItem)
            {
                var pageItem = (PageHamburgerItem)args.Item;
                if (pageItem.PageType != null)
                {
                    if (pageItem.FindPageType)
                        await MoveOrNavigateToPageAsync(pageItem.PageType, pageItem.PageParameter);
                    else
                        await Frame.NavigateAsync(pageItem.PageType, pageItem.PageParameter);
                }
            }
        }
    }
}