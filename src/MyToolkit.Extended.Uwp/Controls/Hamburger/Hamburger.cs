//-----------------------------------------------------------------------
// <copyright file="Hamburger.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
    /// <summary>A Hamburger control.</summary>
    public sealed class Hamburger : Control
    {
        private Button _hamburgerButton;
        private SplitView _splitView;
        private HamburgerItem _previouslySelectedItem;

        /// <summary>Initializes a new instance of the <see cref="Hamburger"/> class.</summary>
        public Hamburger()
        {
            DefaultStyleKey = typeof(Hamburger);

            TopItems = new ObservableCollection<HamburgerItem>();
            BottomItems = new ObservableCollection<HamburgerItem>();
        }

        public static readonly DependencyProperty PaneWidthProperty = DependencyProperty.Register(
            "PaneWidth", typeof(double), typeof(Hamburger), new PropertyMetadata(default(double)));

        /// <summary>Gets or sets the width of the opened pane.</summary>
        public double PaneWidth
        {
            get { return (double)GetValue(PaneWidthProperty); }
            set { SetValue(PaneWidthProperty, value); }
        }

        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(
            "DisplayMode", typeof(SplitViewDisplayMode), typeof(Hamburger), new PropertyMetadata(default(SplitViewDisplayMode)));

        /// <summary>Gets or sets the display mode of the <see cref="SplitView"/>.</summary>
        public SplitViewDisplayMode DisplayMode
        {
            get { return (SplitViewDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        public static readonly DependencyProperty HamburgerBackgroundBrushProperty = DependencyProperty.Register(
            "HamburgerBackgroundBrush", typeof(Brush), typeof(Hamburger), new PropertyMetadata(default(Brush)));

        /// <summary>Gets or sets the background brush of the hamburger button.</summary>
        public Brush HamburgerBackgroundBrush
        {
            get { return (Brush)GetValue(HamburgerBackgroundBrushProperty); }
            set { SetValue(HamburgerBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty HamburgerForegroundBrushProperty = DependencyProperty.Register(
            "HamburgerForegroundBrush", typeof(Brush), typeof(Hamburger), new PropertyMetadata(default(Brush)));

        /// <summary>Gets or sets the foreground brush of the hamburger button.</summary>
        public Brush HamburgerForegroundBrush
        {
            get { return (Brush)GetValue(HamburgerForegroundBrushProperty); }
            set { SetValue(HamburgerForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the content.</summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register(
            "ContentMargin", typeof (Thickness), typeof (Hamburger), new PropertyMetadata(default(Thickness)));

        /// <summary>Gets or sets the content margin.</summary>
        public Thickness ContentMargin
        {
            get { return (Thickness) GetValue(ContentMarginProperty); }
            set { SetValue(ContentMarginProperty, value); }
        }

        public static readonly DependencyProperty TopItemsProperty = DependencyProperty.Register(
            "TopItems", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the top hamburger items.</summary>
        public ObservableCollection<HamburgerItem> TopItems
        {
            get { return (ObservableCollection<HamburgerItem>)GetValue(TopItemsProperty); }
            set { SetValue(TopItemsProperty, value); }
        }

        public static readonly DependencyProperty BottomItemsProperty = DependencyProperty.Register(
            "BottomItems", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the bottom hamburger items.</summary>
        public ObservableCollection<HamburgerItem> BottomItems
        {
            get { return (ObservableCollection<HamburgerItem>)GetValue(BottomItemsProperty); }
            set { SetValue(BottomItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedTopItemProperty = DependencyProperty.Register(
            "SelectedTopItem", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object), (o, args) => ((Hamburger)o).OnSelectedItemChanged((HamburgerItem)args.NewValue)));

        /// <summary>Gets the currently selected top item.</summary>
        public HamburgerItem SelectedTopItem
        {
            get { return (HamburgerItem)GetValue(SelectedTopItemProperty); }
            private set { SetValue(SelectedTopItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedBottomItemProperty = DependencyProperty.Register(
            "SelectedBottomItem", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object), (o, args) => ((Hamburger)o).OnSelectedItemChanged((HamburgerItem)args.NewValue)));

        /// <summary>Gets the currently selected bottom item.</summary>
        public HamburgerItem SelectedBottomItem
        {
            get { return (HamburgerItem)GetValue(SelectedBottomItemProperty); }
            private set { SetValue(SelectedBottomItemProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object)));

        /// <summary>Gets or sets the header control.</summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof(DataTemplate), typeof(Hamburger), new PropertyMetadata(default(DataTemplate)));

        /// <summary>Gets or sets the header template.</summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register(
            "IsPaneOpen", typeof(bool), typeof(Hamburger), new PropertyMetadata(default(bool)));

        /// <summary>Gets or sets a value indicating whether the pane is opened.</summary>
        public bool IsPaneOpen
        {
            get { return (bool)GetValue(IsPaneOpenProperty); }
            set { SetValue(IsPaneOpenProperty, value); }
        }

        /// <summary>Occurs when the hamburger item changed.</summary>
        public event EventHandler<HamburgerItemChangedEventArgs> ItemChanged;

        /// <summary>Gets or sets the currently selected item.</summary>
        public HamburgerItem SelectedItem
        {
            get { return SelectedTopItem ?? SelectedBottomItem; }
            set
            {
                if (TopItems.Contains(value))
                {
                    SelectedTopItem = value;
                    SelectedBottomItem = null;
                }
                else if (BottomItems.Contains(value))
                {
                    SelectedTopItem = null;
                    SelectedBottomItem = value;
                }
                else
                {
                    SelectedTopItem = null;
                    SelectedBottomItem = null;
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _splitView = (SplitView)GetTemplateChild("SplitView");
            _splitView.PaneClosed += (sender, args) =>
            {
                if (IsPaneOpen)
                    IsPaneOpen = false;
            };

            _hamburgerButton = (Button)GetTemplateChild("HamburgerButton");
            _hamburgerButton.Click += OnTogglePane;
        }

        private async void OnSelectedItemChanged(HamburgerItem selectedItem)
        {
            var item = selectedItem;
            if (item != null)
            {
                if (SelectedBottomItem == item)
                    SelectedTopItem = null;
                else
                    SelectedBottomItem = null;

                if (item != _previouslySelectedItem)
                {
                    if (!item.CanBeSelected)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            SelectedItem = _previouslySelectedItem;
                            item.RaiseClickEvent(this);
                        }); // TODO: Improve
                    }
                    else
                    {
                        var copy = ItemChanged;
                        if (copy != null)
                            copy(this, new HamburgerItemChangedEventArgs(item));

                        item.RaiseSelectedEvent(this);
                        item.RaiseClickEvent(this);

                        if (item.AutoClosePane)
                            IsPaneOpen = false;

                        _previouslySelectedItem = item;
                    }
                }
            }
            else
                _previouslySelectedItem = item;
        }

        private void OnTogglePane(object sender, RoutedEventArgs routedEventArgs)
        {
            IsPaneOpen = !IsPaneOpen;
        }
    }
}
