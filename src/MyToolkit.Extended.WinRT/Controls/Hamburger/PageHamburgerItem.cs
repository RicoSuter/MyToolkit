using System;

namespace MyToolkit.Controls
{
    public class PageHamburgerItem : HamburgerItem
    {
        public PageHamburgerItem()
        {
            FindPageType = true;
            AutoClosePane = true; 
        }

        public Type PageType { get; set; }

        public object PageParameter { get; set; }

        /// <summary>Gets or sets a value indicating whether to search for page in the current page stack (default: true).</summary>
        public bool FindPageType { get; set; }
    }
}