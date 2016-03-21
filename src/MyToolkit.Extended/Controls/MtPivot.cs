//-----------------------------------------------------------------------
// <copyright file="MtPivot.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace MyToolkit.Controls
{
    /// <summary>A pivot control for WinRT. </summary>
    [ContentProperty(Name = "Items")]
    public class MtPivot : Control
    {
        private MtListBox _list;
        private Grid _grid;
        private Storyboard _story;
        private TranslateTransform _translate;

        private int _initialIndex = 0;
        private object _initialItem;

        private readonly ObservableCollection<MtPivotItem> _items = new ObservableCollection<MtPivotItem>();

        /// <summary>Initializes a new instance of the <see cref="MtPivot"/> class. </summary>
        public MtPivot()
        {
            DefaultStyleKey = typeof(MtPivot);
        }

        public static readonly DependencyProperty IsHeaderEnabledProperty = DependencyProperty.Register(
            "IsHeaderEnabled", typeof(bool), typeof(MtPivot), new PropertyMetadata(true));
        
        /// <summary>Gets or sets a value indicating whether the header is enabled and can be interacted with.</summary>
        public bool IsHeaderEnabled
        {
            get { return (bool)GetValue(IsHeaderEnabledProperty); }
            set { SetValue(IsHeaderEnabledProperty, value); }
        }

        /// <summary>Occurs when the selected pivot changed. </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>Gets the list of <see cref="MtPivotItem"/> objects. </summary>
        public ObservableCollection<MtPivotItem> Items { get { return _items; } }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(MtPivot), new PropertyMetadata(default(DataTemplate)));

        /// <summary>Gets or sets the header template. </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(MtPivot),
            new PropertyMetadata(default(object), (o, args) => ((MtPivot)o).OnSelectedItemChanged()));

        /// <summary>Gets or sets the current visible <see cref="MtPivotItem"/>. </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(MtPivot),
            new PropertyMetadata(default(int), (o, args) => ((MtPivot)o).OnSelectedIndexChanged()));

        /// <summary>Gets or sets the index of the currently selected <see cref="MtPivotItem"/>. </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush", typeof(Brush), typeof(MtPivot), new PropertyMetadata(default(Brush)));

        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        public static readonly DependencyProperty UnselectedBrushProperty = DependencyProperty.Register(
            "UnselectedBrush", typeof(Brush), typeof(MtPivot), new PropertyMetadata(default(Brush)));

        public Brush UnselectedBrush
        {
            get { return (Brush)GetValue(UnselectedBrushProperty); }
            set { SetValue(UnselectedBrushProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _list = (MtListBox)GetTemplateChild("List");
            _grid = (Grid)GetTemplateChild("Grid");

            _translate = (TranslateTransform)GetTemplateChild("Translate");
            _story = (Storyboard)GetTemplateChild("Story");

            _list.ItemsSource = Items;
            _list.SelectionChanged += OnSelectionChanged;

            foreach (var item in Items.Where(i => i.Preload))
            {
                item.Content.Visibility = Visibility.Collapsed;
                AddElement(item.Content);
            }

            _items.CollectionChanged += OnCollectionChanged;

            if (_initialItem != null)
                SelectedItem = _initialItem;
            else if (Items.Count > _initialIndex)
                SelectedItem = _items[_initialIndex];
            else
                SelectedItem = _items.FirstOrDefault();
        }

        private FrameworkElement CurrentPivotElement
        {
            get { return _list != null && _list.SelectedItem != null ? ((MtPivotItem)_list.SelectedItem).Content : null; }
        }

        private void OnSelectedItemChanged()
        {
            if (_list != null)
                _list.SelectedItem = SelectedItem;
            else
                _initialItem = SelectedItem;
        }

        private void OnSelectedIndexChanged()
        {
            if (_list != null)
                _list.SelectedIndex = SelectedIndex;
            else
                _initialIndex = SelectedIndex;
        }

        private void AddElement(FrameworkElement element)
        {
            if (!_grid.Children.Contains(element))
                _grid.Children.Add(element);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
            {
                foreach (MtPivotItem item in args.OldItems)
                {
                    if (!_items.Contains(item) && _grid.Children.Contains(item.Content))
                        _grid.Children.Remove(item.Content);
                }
            }

            if (args.NewItems != null)
            {
                foreach (MtPivotItem item in args.NewItems)
                {
                    if (_items.Contains(item) && item.Preload)
                        AddElement(item.Content);
                }
            }

            SelectedItem = _items.FirstOrDefault();
        }

        private void ShowSelectedPivot()
        {
            _translate.X = 30;
            CurrentPivotElement.Visibility = Visibility.Visible;
            _story.Begin();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (CurrentPivotElement == null)
            {
                SelectedIndex = 0;
                return;
            }

            foreach (var item in _grid.Children)
                item.Visibility = Visibility.Collapsed;

            AddElement(CurrentPivotElement);

            SelectedIndex = _list.SelectedIndex;
            SelectedItem = _list.SelectedItem;

            var copy = SelectionChanged;
            if (copy != null)
                copy(sender, args);

            ShowSelectedPivot();
        }
    }

    [Obsolete("Use MtPivot instead. 8/31/2014")]
    public class Pivot : MtPivot
    {
    }

    public class ProxyElement : FrameworkElement
    {
    }
}

#endif