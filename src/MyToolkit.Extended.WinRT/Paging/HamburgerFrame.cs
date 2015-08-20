using System.Linq;
using MyToolkit.Controls;

namespace MyToolkit.Paging
{
    public class HamburgerFrame
    {
        public HamburgerFrame()
        {
            Frame = new MtFrame();
            Frame.PageAnimation = null; 
            Frame.Navigated += FrameOnNavigated;

            Hamburger = new Hamburger();
            Hamburger.Content = Frame; 
            Hamburger.ItemChanged += HamburgerOnItemChanged;
        }

        public MtFrame Frame { get; private set; }

        public Hamburger Hamburger { get; set; }

        private void FrameOnNavigated(object sender, MtNavigationEventArgs args)
        {
            var currentItem = Hamburger.TopItems
                .Concat(Hamburger.BottomItems)
                .OfType<PageHamburgerItem>()
                .FirstOrDefault(i => i.PageType == Frame.CurrentPage.Type); 

            Hamburger.CurrentItem = currentItem;
        }

        private async void HamburgerOnItemChanged(object sender, HamburgerItemChangedEventArgs args)
        {
            if (args.Item is PageHamburgerItem)
            {
                var pageItem = (PageHamburgerItem)args.Item;
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