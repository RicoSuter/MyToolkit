//-----------------------------------------------------------------------
// <copyright file="PopupHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using MyToolkit.Events;
using MyToolkit.Utilities;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace MyToolkit.UI
{
    public static class PopupHelper
    {
        /// <summary>
        /// Gets a value indicating whether a popup is currently visible. 
        /// </summary>
        public static bool IsPopupVisible
        {
            get { return VisualTreeHelper.GetOpenPopups(Window.Current).Any(); }
        }

        /// <summary>
        /// Gets the parent popup of the given element or null if it is not contained in a popup. 
        /// </summary>
        public static Popup GetParentPopup(FrameworkElement element)
        {
            return element.GetVisualAncestors().LastOrDefault() as Popup; 
        }

        /// <summary>
        /// Returns true if the element is contained in a popup. 
        /// </summary>
        public static bool IsInPopup(FrameworkElement element)
        {
            if (element is Popup)
                return true;

            return GetParentPopup(element) != null; 
        }

        /// <summary>
        /// Closes the parent popup of the given control. 
        /// </summary>
        /// <param name="childControl">The child control. </param>
        public static void ClosePopup(this FrameworkElement childControl)
        {
            ((Popup) childControl.Tag).IsOpen = false; 
        }

        /// <summary>
        /// Shows a horizontal popup given as <see cref="FrameworkElement"/> and provides a task to wait until it is closed. 
        /// </summary>
        public static Task<Popup> ShowHorizontalDialogAsync(FrameworkElement control, bool isLightDismissEnabled = false)
        {
            return TaskUtilities.RunCallbackMethodAsync<FrameworkElement, Popup>((x, y) => ShowDialog(x, isLightDismissEnabled, true, y), control);
        }

        /// <summary>
        /// Shows a vertical popup given as <see cref="FrameworkElement"/> and provides a task to wait until it is closed. 
        /// </summary>
        public static Task<Popup> ShowVerticalDialogAsync(FrameworkElement control, bool isLightDismissEnabled = false)
        {
            return TaskUtilities.RunCallbackMethodAsync<FrameworkElement, Popup>((x, y) => ShowDialog(x, isLightDismissEnabled, false, y), control);
        }

        /// <summary>
        /// Shows a horizontal popup given as <see cref="FrameworkElement"/> and provides a task to wait until it is closed. 
        /// </summary>
        [Obsolete("Use the ShowHorizontalDialogAsync method instead. 7/17/2014")]
        public static Popup ShowHorizontalDialog(FrameworkElement control, bool isLightDismissEnabled = false, Action<Popup> closedCallback = null)
        {
            return ShowDialog(control, isLightDismissEnabled, true, closedCallback);
        }

        /// <summary>
        /// Shows a vertical popup given as <see cref="FrameworkElement"/> and provides a task to wait until it is closed. 
        /// </summary>
        [Obsolete("Use the ShowVerticalDialogAsync method instead. 7/17/2014")]
        public static Popup ShowVerticalDialog(FrameworkElement control, bool isLightDismissEnabled = false, Action<Popup> closedCallback = null)
        {
            return ShowDialog(control, isLightDismissEnabled, false, closedCallback);
        }

        private static Popup ShowDialog(FrameworkElement control, bool isLightDismissEnabled = false, bool isHorizontal = true, Action<Popup> closedCallback = null)
        {
            var popup = new Popup();
            var parent = (FrameworkElement)Window.Current.Content;

            var windowActivated = new WindowActivatedEventHandler((sender, e) =>
            {
                UpdatePopupControlSize(control, isHorizontal);
                UpdatePopupOffsets(control, isHorizontal, popup);
            });

            var controlSizeChanged = new SizeChangedEventHandler((sender, e) => 
                UpdatePopupOffsets(control, isHorizontal, popup));

            Window.Current.Activated += windowActivated;
            UpdatePopupControlSize(control, isHorizontal);

            control.SizeChanged += controlSizeChanged;
            control.Tag = popup; 

            var oldOpacity = parent.Opacity;
            parent.Opacity = 0.5; 
            parent.IsHitTestVisible = false;

            var topAppBarVisibility = Visibility.Collapsed;
            var bottomAppBarVisibility = Visibility.Collapsed;

            if (parent is Paging.MtFrame)
            {
                var page = ((Paging.MtFrame)parent).CurrentPage.Page;
                if (page != null)
                {
                    if (page.TopAppBar != null)
                    {
                        topAppBarVisibility = page.TopAppBar.Visibility;
                        page.TopAppBar.Visibility = Visibility.Collapsed;
                    }
                    if (page.BottomAppBar != null)
                    {
                        bottomAppBarVisibility = page.BottomAppBar.Visibility;
                        page.BottomAppBar.Visibility = Visibility.Collapsed;
                    }
                }
            } 
            else if (parent is Frame)
            {
                var page = ((Frame)parent).Content as Page;
                if (page != null)
                {
                    if (page.TopAppBar != null)
                    {
                        topAppBarVisibility = page.TopAppBar.Visibility;
                        page.TopAppBar.Visibility = Visibility.Collapsed;
                    }
                    if (page.BottomAppBar != null)
                    {
                        bottomAppBarVisibility = page.BottomAppBar.Visibility;
                        page.BottomAppBar.Visibility = Visibility.Collapsed;
                    }
                }
            }

            popup.Child = control;
            popup.IsLightDismissEnabled = isLightDismissEnabled;
            popup.Closed += delegate
            {
                parent.Opacity = oldOpacity; 
                parent.IsHitTestVisible = true;

                if (parent is Paging.MtFrame)
                {
                    var page = ((Paging.MtFrame)parent).CurrentPage.Page;
                    if (page != null)
                    {
                        if (page.TopAppBar != null)
                            page.TopAppBar.Visibility = topAppBarVisibility;
                        if (page.BottomAppBar != null)
                            page.BottomAppBar.Visibility = bottomAppBarVisibility;
                    }
                }
                else if (parent is Frame)
                {
                    var page = ((Frame)parent).Content as Page;
                    if (page != null)
                    {
                        if (page.TopAppBar != null)
                            page.TopAppBar.Visibility = topAppBarVisibility;
                        if (page.BottomAppBar != null)
                            page.BottomAppBar.Visibility = bottomAppBarVisibility;
                    }
                }

                Window.Current.Activated -= windowActivated;
                control.SizeChanged -= controlSizeChanged;

                if (closedCallback != null)
                    closedCallback(popup);
            };
            popup.IsOpen = true;

            popup.Tag = 0.0;
            InputPane.GetForCurrentView().Showing += (s, args) => UpdateElementLocation(popup);
            InputPane.GetForCurrentView().Hiding += (s, args) =>
            {
                popup.VerticalOffset += (double)popup.Tag;
                popup.Tag = 0.0;
            };
            return popup;
        }

        private static void UpdatePopupOffsets(FrameworkElement control, bool isHorizontal, Popup popup)
        {
            if (isHorizontal)
                popup.VerticalOffset = Window.Current.Bounds.Top + (Window.Current.Bounds.Height - control.ActualHeight)/2;
            else
                popup.HorizontalOffset = Window.Current.Bounds.Left + (Window.Current.Bounds.Width - control.ActualWidth)/2;
        }

        private static void UpdatePopupControlSize(FrameworkElement control, bool isHorizontal)
        {
            if (isHorizontal)
                control.Width = Window.Current.Bounds.Width;
            else
                control.Height = Window.Current.Bounds.Height;
        }

        private static void UpdateElementLocation(Popup popup)
        {
            var occlutedRect = InputPane.GetForCurrentView().OccludedRect;
            if (occlutedRect.Top > 0)
            {
                var element = FocusManager.GetFocusedElement() as FrameworkElement;
                if (element != null)
                {
                    SingleEvent.RegisterEvent(element,
                        (e, h) => e.LostFocus += h,
                        (e, h) => e.LostFocus -= h,
                        delegate { UpdateElementLocation(popup); });

                    var point = element.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
                    if (point.X + element.ActualHeight + 100 > occlutedRect.Top)
                    {
                        var offset = (point.X + element.ActualHeight + 100) - occlutedRect.Top - (double)popup.Tag;
                        if (offset > 20)
                        {
                            popup.VerticalOffset -= offset;
                            popup.Tag = (double)popup.Tag + offset;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shows a <see cref="FrameworkElement"/> as settings popup. 
        /// Obsolete: Use the <see cref="SettingsFlyout"/> control instead. 
        /// </summary>
        [Obsolete("Use the SettingsFlyout control instead. 5/17/2014")]
        public static Popup ShowSettings(FrameworkElement control, Action<Popup> closedCallback = null)
        {
            var bounds = Window.Current.Bounds;
            control.Height = bounds.Height;

            var popup = new Popup();
            var del1 = new WindowActivatedEventHandler((sender, e) =>
            {
                if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
                    popup.IsOpen = false;
            });
            var del2 = new SizeChangedEventHandler((sender, e) =>
            {
                popup.HorizontalOffset = bounds.Left + (bounds.Width - control.ActualWidth);
            });

            Window.Current.Activated += del1;
            control.SizeChanged += del2;

            popup.IsLightDismissEnabled = true;
            popup.Child = control;
            popup.Closed += delegate 
            {
                Window.Current.Activated -= del1;
                control.SizeChanged -= del2;
                if (closedCallback != null)
                    closedCallback(popup);
            };
            popup.IsOpen = true;
            return popup;
        }

        /// <summary>
        /// Shows a <see cref="FrameworkElement"/> as pane. 
        /// </summary>
        public static Task<Popup> ShowPaneAsync(FrameworkElement control, bool showLeft = true)
        {
            return TaskUtilities.RunCallbackMethodAsync<FrameworkElement, Popup>((x, y) => ShowPane(x, showLeft, y), control);
        }

        /// <summary>
        /// Shows a <see cref="FrameworkElement"/> as pane. 
        /// </summary>
        [Obsolete("Use the ShowPaneAsync method instead. 7/17/2014")]
        public static Popup ShowPane(FrameworkElement control, bool showLeft = true, Action<Popup> closedCallback = null)
        {
            var bounds = Window.Current.Bounds;
            control.Height = bounds.Height;

            var popup = new Popup();
            var del = new WindowActivatedEventHandler((sender, e) =>
            {
                if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
                    popup.IsOpen = false;
            });
            var del2 = new SizeChangedEventHandler((sender, e) =>
            {
                if (!showLeft)
                    popup.HorizontalOffset = bounds.Left + (bounds.Width - control.ActualWidth);
            });

            Window.Current.Activated += del;
            control.SizeChanged += del2;

            popup.IsLightDismissEnabled = true;
            popup.Child = control;
            popup.Closed += delegate
            {
                Window.Current.Activated -= del;
                control.SizeChanged -= del2;
                if (closedCallback != null)
                    closedCallback(popup);
            };
            popup.IsOpen = true;
            return popup;
        }
    }
}

#endif