//-----------------------------------------------------------------------
// <copyright file="HamburgerItem.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
    /// <summary>A hamburger item.</summary>
    public class HamburgerItem : DependencyObject
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(object), typeof(HamburgerItem), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the icon which is shown when the pane is closed.</summary>
        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty ContentIconProperty = DependencyProperty.Register(
            "ContentIcon", typeof(object), typeof(HamburgerItem), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the content icon which is shown when the pane is open.</summary>
        public object ContentIcon
        {
            get { return (object)GetValue(ContentIconProperty); }
            set { SetValue(ContentIconProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(HamburgerItem), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the content which is shown when the pane is open.</summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty AutoClosePaneProperty = DependencyProperty.Register(
            "AutoClosePane", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(default(bool)));

        /// <summary>Gets or sets a value indicating whether to automatic close pane when the item is selected.</summary>
        public bool AutoClosePane
        {
            get { return (bool)GetValue(AutoClosePaneProperty); }
            set { SetValue(AutoClosePaneProperty, value); }
        }

        public static readonly DependencyProperty ShowContentIconProperty = DependencyProperty.Register(
            "ShowContentIcon", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(true));

        /// <summary>Gets or sets a value indicating whether to show the content icon when the pane is open.</summary>
        public bool ShowContentIcon
        {
            get { return (bool)GetValue(ShowContentIconProperty); }
            set { SetValue(ShowContentIconProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(true));

        /// <summary>Gets or sets a value indicating whether the item is enabled.</summary>
        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty CanBeSelectedProperty = DependencyProperty.Register(
            "CanBeSelected", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(true));

        /// <summary>Gets or sets a value indicating whether the item can be selected.</summary>
        public bool CanBeSelected
        {
            get { return (bool)GetValue(CanBeSelectedProperty); }
            set { SetValue(CanBeSelectedProperty, value); }
        }

        /// <summary>Occurs when the item has been selected.</summary>
        public event EventHandler<HamburgerItemSelectedEventArgs> Selected;

        /// <summary>Occurs when the item has been clicked (may not be selected when <see cref="CanBeSelected"/> is false).</summary>
        public event EventHandler<HamburgerItemClickedEventArgs> Click;

        internal void RaiseSelectedEvent(Hamburger hamburger)
        {
            var copy = Selected;
            if (copy != null)
                copy(this, new HamburgerItemSelectedEventArgs(hamburger));
        }

        internal void RaiseClickEvent(Hamburger hamburger)
        {
            var copy = Click;
            if (copy != null)
                copy(this, new HamburgerItemClickedEventArgs(hamburger));
        }
    }
}
