using System;

namespace MyToolkit.Controls
{
    public class HamburgerItemSelectedEventArgs : EventArgs
    {
        public HamburgerItemSelectedEventArgs(Hamburger hamburger)
        {
            Hamburger = hamburger;
        }

        public Hamburger Hamburger { get; private set; }
    }
}