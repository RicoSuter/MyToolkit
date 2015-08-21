//-----------------------------------------------------------------------
// <copyright file="HamburgerFrameFactory.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using MyToolkit.Controls;

namespace MyToolkit.Paging
{
    /// <summary>A factory to create a <see cref="Hamburger"/> which works in conjunction with a <see cref="MtFrame"/>.</summary>
    public class HamburgerFrameFactory
    {
        /// <summary>Initializes a new instance of the <see cref="HamburgerFrameFactory"/> class.</summary>
        public HamburgerFrameFactory()
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
                    {
                        var existingPage = Frame.GetNearestPageOfTypeInBackStack(pageItem.PageType);
                        if (existingPage != null)
                            await Frame.MovePageToTopOfStackAsync(existingPage);
                        else
                            await Frame.NavigateAsync(pageItem.PageType, pageItem.PageParameter);
                    }
                    else
                        await Frame.NavigateAsync(pageItem.PageType, pageItem.PageParameter);
                }
            }
        }
    }
}