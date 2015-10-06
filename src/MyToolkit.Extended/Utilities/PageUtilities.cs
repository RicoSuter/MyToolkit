//-----------------------------------------------------------------------
// <copyright file="PageUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using MyToolkit.Events;
using MyToolkit.UI;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Utilities
{
    /// <summary>Provides utility methods for page handling. </summary>
    public static class PageUtilities
    {
        /// <summary>Call this method in Loaded event as the event will be automatically 
        /// deregistered when the FrameworkElement has been unloaded. </summary>
        public static void RegisterBackKey(Page page)
        {
            var callback = new TypedEventHandler<CoreDispatcher, AcceleratorKeyEventArgs>(
                delegate(CoreDispatcher sender, AcceleratorKeyEventArgs args)
                {
                    if (!args.Handled && args.VirtualKey == VirtualKey.Back &&
                        (args.EventType == CoreAcceleratorKeyEventType.KeyDown || 
                            args.EventType == CoreAcceleratorKeyEventType.SystemKeyDown))
                    {
                        var element = FocusManager.GetFocusedElement();
                        if (element is FrameworkElement && PopupHelper.IsInPopup((FrameworkElement)element))
                            return;

                        if (element is TextBox || element is PasswordBox || element is WebView)
                            return; 

                        if (page.Frame.CanGoBack)
                        {
                            args.Handled = true;
                            page.Frame.GoBack();
                        }
                    }
                });

            page.Dispatcher.AcceleratorKeyActivated += callback;

            SingleEvent.RegisterEvent(page,
                (p, h) => p.Unloaded += h,
                (p, h) => p.Unloaded -= h,
                (o, a) => { page.Dispatcher.AcceleratorKeyActivated -= callback; });
        }

        /// <summary>Call this method in Loaded event as the event will be automatically 
        /// deregistered when the FrameworkElement has been unloaded. </summary>
        /// <param name="page">The page. </param>
        /// <param name="handler">The event handler. </param>
        public static void RegisterAcceleratorKeyActivated(FrameworkElement page, TypedEventHandler<CoreDispatcher, AcceleratorKeyEventArgs> handler)
        {
            page.Dispatcher.AcceleratorKeyActivated += handler;
            SingleEvent.RegisterEvent(page, (p, h) => p.Unloaded += h, (p, h) => p.Unloaded -= h, (o, a) =>
            {
                page.Dispatcher.AcceleratorKeyActivated -= handler;
            });
        }

        /// <summary>Call this method in Loaded event as the event will be automatically 
        /// deregistered when the FrameworkElement has been unloaded. </summary>
        public static void RegisterSearchPressed(FrameworkElement page, Action searchKeyPressed)
        {
            var c = new SearchKeyContainer(searchKeyPressed);
            RegisterAcceleratorKeyActivated(page, c.AcceleratorKeyActivated);
        }

        internal class SearchKeyContainer
        {
            private readonly Action _searchKeyPressed;
            private bool _controlDown = false;

            public SearchKeyContainer(Action searchKeyPressed)
            {
                _searchKeyPressed = searchKeyPressed;
            }

            public void AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
            {
                if (args.VirtualKey == VirtualKey.Control)
                    _controlDown = args.EventType == CoreAcceleratorKeyEventType.KeyDown;

                if (args.VirtualKey == VirtualKey.F3 || (args.VirtualKey == VirtualKey.F && _controlDown))
                {
                    _searchKeyPressed();
                    args.Handled = true;
                }
            }
        }
    }
}

#endif