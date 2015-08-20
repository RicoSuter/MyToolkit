using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using MyToolkit.Command;

namespace MyToolkit.Controls
{
    public sealed class Hamburger : Control
    {
        private RadioButton _hamburgerButton;
        private SplitView _splitView;
        private HamburgerItem _currentItem;
        private bool _isChanging;

        public Hamburger()
        {
            DefaultStyleKey = typeof(Hamburger);

            TopItems = new ObservableCollection<HamburgerItem>();
            BottomItems = new ObservableCollection<HamburgerItem>();
        }

        public static readonly DependencyProperty PaneWidthProperty = DependencyProperty.Register(
            "PaneWidth", typeof (double), typeof (Hamburger), new PropertyMetadata(default(double)));

        public double PaneWidth
        {
            get { return (double) GetValue(PaneWidthProperty); }
            set { SetValue(PaneWidthProperty, value); }
        }

        public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register(
            "DisplayMode", typeof (SplitViewDisplayMode), typeof (Hamburger), new PropertyMetadata(default(SplitViewDisplayMode)));

        public SplitViewDisplayMode DisplayMode
        {
            get { return (SplitViewDisplayMode) GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        public static readonly DependencyProperty HamburgerBackgroundBrushProperty = DependencyProperty.Register(
            "HamburgerBackgroundBrush", typeof (Brush), typeof (Hamburger), new PropertyMetadata(default(Brush)));

        public Brush HamburgerBackgroundBrush
        {
            get { return (Brush) GetValue(HamburgerBackgroundBrushProperty); }
            set { SetValue(HamburgerBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty HamburgerForegroundBrushProperty = DependencyProperty.Register(
            "HamburgerForegroundBrush", typeof (Brush), typeof (Hamburger), new PropertyMetadata(default(Brush)));

        public Brush HamburgerForegroundBrush
        {
            get { return (Brush) GetValue(HamburgerForegroundBrushProperty); }
            set { SetValue(HamburgerForegroundBrushProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object)));

        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty TopItemsProperty = DependencyProperty.Register(
            "TopItems", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object)));
        
        public ObservableCollection<HamburgerItem> TopItems
        {
            get { return (ObservableCollection<HamburgerItem>)GetValue(TopItemsProperty); }
            set { SetValue(TopItemsProperty, value); }
        }

        public static readonly DependencyProperty BottomItemsProperty = DependencyProperty.Register(
            "BottomItems", typeof(object), typeof(Hamburger), new PropertyMetadata(default(object)));

        public ObservableCollection<HamburgerItem> BottomItems
        {
            get { return (ObservableCollection<HamburgerItem>)GetValue(BottomItemsProperty); }
            set { SetValue(BottomItemsProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header", typeof (object), typeof (Hamburger), new PropertyMetadata(default(object)));

        public object Header
        {
            get { return (object) GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", typeof (DataTemplate), typeof (Hamburger), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty IsPaneOpenProperty = DependencyProperty.Register(
            "IsPaneOpen", typeof (bool), typeof (Hamburger), new PropertyMetadata(default(bool)));

        public bool IsPaneOpen
        {
            get { return (bool) GetValue(IsPaneOpenProperty); }
            set { SetValue(IsPaneOpenProperty, value); }
        }

        public event EventHandler<HamburgerItemChangedEventArgs> ItemChanged;

        public HamburgerItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (_isChanging)
                    return;

                if (_currentItem != value)
                {
                    _currentItem = value;
                    _isChanging = true; 

                    foreach (var item in TopItems.Concat(BottomItems))
                        item.IsSelected = false;

                    if (_currentItem != null)
                    {
                        foreach (var item in TopItems.Concat(BottomItems).Where(i => i == _currentItem))
                            item.IsSelected = true;
                    }

                    _isChanging = false;
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _splitView = (SplitView)GetTemplateChild("SplitView");
            _splitView.Tag = new RelayCommand<HamburgerItem>(RaiseItemChanged);
            _splitView.PaneClosed += (sender, args) =>
            {
                if (IsPaneOpen)
                    IsPaneOpen = false;
            };

            _hamburgerButton = (RadioButton)GetTemplateChild("HamburgerButton");
            _hamburgerButton.Click += OnTogglePane;
        }

        private void OnTogglePane(object sender, RoutedEventArgs routedEventArgs)
        {
            IsPaneOpen = !IsPaneOpen;
        }

        private void RaiseItemChanged(HamburgerItem item)
        {
            if (_isChanging)
                return; 

            if (item != CurrentItem)
            {
                CurrentItem = item;

                var copy = ItemChanged;
                if (copy != null)
                    copy(this, new HamburgerItemChangedEventArgs(item));

                item.RaiseSelected(this);
            }

            if (item.AutoClosePane)
                IsPaneOpen = false;
        }
    }
}
