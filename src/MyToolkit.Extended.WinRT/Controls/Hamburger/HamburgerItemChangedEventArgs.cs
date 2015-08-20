using System;

namespace MyToolkit.Controls
{
    public class HamburgerItemChangedEventArgs : EventArgs
    {
        public HamburgerItemChangedEventArgs(HamburgerItem item)
        {
            Item = item;
        }

        public HamburgerItem Item { get; private set; }
    }
}