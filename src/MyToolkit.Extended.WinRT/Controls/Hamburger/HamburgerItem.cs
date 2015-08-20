using System;
using Windows.UI.Xaml;

namespace MyToolkit.Controls
{
    public class HamburgerItem : DependencyObject
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(object), typeof(HamburgerItem), new PropertyMetadata(default(object)));

        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public static readonly DependencyProperty ContentIconProperty = DependencyProperty.Register(
            "ContentIcon", typeof(object), typeof(HamburgerItem), new PropertyMetadata(default(object)));

        public object ContentIcon
        {
            get { return (object)GetValue(ContentIconProperty); }
            set { SetValue(ContentIconProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(HamburgerItem), new PropertyMetadata(default(object)));
        
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty AutoClosePaneProperty = DependencyProperty.Register(
            "AutoClosePane", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(default(bool)));

        public bool AutoClosePane
        {
            get { return (bool)GetValue(AutoClosePaneProperty); }
            set { SetValue(AutoClosePaneProperty, value); }
        }

        public static readonly DependencyProperty ShowContentIconProperty = DependencyProperty.Register(
            "ShowContentIcon", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(true));

        public bool ShowContentIcon
        {
            get { return (bool)GetValue(ShowContentIconProperty); }
            set { SetValue(ShowContentIconProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(true));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty CanBeSelectedProperty = DependencyProperty.Register(
            "CanBeSelected", typeof(bool), typeof(HamburgerItem), new PropertyMetadata(true));

        public bool CanBeSelected
        {
            get { return (bool)GetValue(CanBeSelectedProperty); }
            set { SetValue(CanBeSelectedProperty, value); }
        }

        public event EventHandler<HamburgerItemSelectedEventArgs> Selected;

        internal void RaiseSelected(Hamburger hamburger)
        {
            var copy = Selected;
            if (copy != null)
                copy(this, new HamburgerItemSelectedEventArgs(hamburger));
        }
    }
}
