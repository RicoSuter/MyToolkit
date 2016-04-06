//-----------------------------------------------------------------------
// <copyright file="DataGrid.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using MyToolkit.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MyToolkit.UI;
using MyToolkit.Utilities;

namespace MyToolkit.Controls
{
    /// <summary>A data grid control. </summary>
    public class DataGrid : Control
    {
        private Grid _titleRowControl;
        private MtListBox _listControl;
        private bool _initialized = false;
        private object _initialSelectedItem;
        private object _initialFilter;

        /// <summary>Initializes a new instance of the <see cref="DataGrid"/> class. </summary>
        public DataGrid()
        {
            DefaultStyleKey = typeof(DataGrid);
            Columns = new DataGridColumnCollection(); // Initialize collection so that columns can be defined in XAML

            if (!Designer.IsInDesignMode)
                Loaded += OnLoaded;
        }

        /// <summary>Occurs when the selected item (row) has changed. </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>Occurs when the order (column) has changed. </summary>
        public event EventHandler<DataGridColumnEventArgs> OrderChanged;

        /// <summary>Occurs when the user clicked on an item and wants to navigate to its detail page. </summary>
        public event EventHandler<NavigationListEventArgs> Navigate;

        /// <summary>Gets the list of currently selected items. </summary>
        public IList<object> SelectedItems
        {
            get { return _listControl.SelectedItems; }
        }

        /// <summary>Gets the currently displayed items. </summary>
        public IObservableCollectionView Items
        {
            get { return _listControl == null ? null : _listControl.ItemsSource as IObservableCollectionView; }
        }

        /// <summary>Gets the selected column. </summary>
        public DataGridColumnBase SelectedColumn { get; private set; }

        #region Dependency Properties

        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(DataGrid), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(DataGrid), new PropertyMetadata(SelectionMode.Single));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(DataGrid), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(DataGrid),
                new PropertyMetadata(null, (obj, args) => ((DataGrid)obj).UpdateItemsSource()));

        public static readonly DependencyProperty DefaultOrderIndexProperty =
            DependencyProperty.Register("DefaultOrderIndex", typeof(int), typeof(DataGrid), new PropertyMetadata(-1));

        public static readonly DependencyProperty RowStyleProperty =
            DependencyProperty.Register("RowStyle", typeof(Style), typeof(DataGrid), new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty ItemDetailsTemplateProperty =
            DependencyProperty.Register("ItemDetailsTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns",
            typeof(ObservableCollection<DataGridColumnBase>), typeof(DataGrid), new PropertyMetadata(null, OnColumnsPropertyChanged));

        public static readonly DependencyProperty CellTemplateProperty = 
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));

        /// <summary>Gets or sets the header background. </summary>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        /// <summary>Gets or sets the selection mode (default: single). </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>Gets or sets the selected item. </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>Gets or sets the items collection to show in the <see cref="DataGrid"/>.</summary>
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>Gets or sets the index of the column which is initially ordered.</summary>
        public int DefaultOrderIndex
        {
            get { return (int)GetValue(DefaultOrderIndexProperty); }
            set { SetValue(DefaultOrderIndexProperty, value); }
        }

        /// <summary>Used to change the row style, the ItemContainerStyle of the internal ListBox; use ListBoxItem as style target type.</summary>
        public Style RowStyle
        {
            get { return (Style)GetValue(RowStyleProperty); }
            set { SetValue(RowStyleProperty, value); }
        }

        /// <summary>Gets or sets the data template for item details (shown when an item is selected). When null then no details are shown.</summary>
        public DataTemplate ItemDetailsTemplate
        {
            get { return (DataTemplate)GetValue(ItemDetailsTemplateProperty); }
            set { SetValue(ItemDetailsTemplateProperty, value); }
        }

        /// <summary>Gets or sets the header data template (styling of column container).</summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>Gets or sets the cell data template (styling of cell container).</summary>
        public DataTemplate CellTemplate
        {
            get { return (DataTemplate) GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        /// <summary>Gets the column description of the <see cref="DataGrid"/>. </summary>
        public DataGridColumnCollection Columns
        {
            get { return (DataGridColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty RowBackgroundOddBrushProperty = DependencyProperty.Register(
            "RowBackgroundOddBrush", typeof(Brush), typeof(DataGrid), new PropertyMetadata(default(Brush)));

        public Brush RowBackgroundOddBrush
        {
            get { return (Brush)GetValue(RowBackgroundOddBrushProperty); }
            set { SetValue(RowBackgroundOddBrushProperty, value); }
        }

        public static readonly DependencyProperty RowBackgroundEvenBrushProperty = DependencyProperty.Register(
            "RowBackgroundEvenBrush", typeof (Brush), typeof (DataGrid), new PropertyMetadata(default(Brush)));

        public Brush RowBackgroundEvenBrush
        {
            get { return (Brush) GetValue(RowBackgroundEvenBrushProperty); }
            set { SetValue(RowBackgroundEvenBrushProperty, value); }
        }

        #endregion

        /// <summary>Selects a column for ordering. 
        /// If the column is not selected the the default ordering is used (IsAscendingDefault property). 
        /// If the column is already selected then the ordering direction will be inverted. </summary>
        /// <param name="column">The column. </param>
        /// <returns>Returns true if column could be changed. </returns>
        public bool SelectColumn(DataGridColumnBase column)
        {
            return SelectColumn(column, column == SelectedColumn ? !column.IsAscending : column.IsAscendingDefault);
        }

        /// <summary>Selects a column for ordering. </summary>
        /// <param name="column">The column. </param>
        /// <param name="ascending">The value indicating whether to sort the column ascending; otherwise descending. </param>
        /// <returns>Returns true if column could be changed. </returns>
        public bool SelectColumn(DataGridColumnBase column, bool ascending)
        {
            if (column.CanSort)
            {
                var oldColumn = SelectedColumn;
                if (oldColumn != null)
                    oldColumn.IsSelected = false;

                SelectedColumn = column;
                SelectedColumn.IsSelected = true;
                SelectedColumn.IsAscending = ascending;

                OnOrderChanged(this, new DataGridColumnEventArgs(SelectedColumn));
                return true;
            }
            return false;
        }

        /// <summary>Sets the filter on the items source. </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="filter"></param>
        public void SetFilter<TItem>(Func<TItem, bool> filter)
        {
            if (_listControl == null)
                _initialFilter = filter;
            else
            {
                if (Items != null)
                    Items.Filter = filter;
            }
        }

        /// <summary>Removes the current filter. </summary>
        public void RemoveFilter()
        {
            if (Items != null)
                Items.Filter = null;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _titleRowControl = (Grid)GetTemplateChild("ColumnHeaders");

            _listControl = (MtListBox)GetTemplateChild("Rows");
            _listControl.PrepareContainerForItem += OnPrepareContainerForItem;
            _listControl.SelectionChanged += OnSelectionChanged;

            _listControl.SetBinding(Selector.SelectedItemProperty,
                new Binding { Source = this, Path = new PropertyPath("SelectedItem"), Mode = BindingMode.TwoWay });
            _listControl.SetBinding(ListBox.SelectionModeProperty,
                new Binding { Source = this, Path = new PropertyPath("SelectionMode"), Mode = BindingMode.TwoWay });

            _initialSelectedItem = SelectedItem;

            if (DefaultOrderIndex == -1)
            {
                var currentOrdered = Columns.FirstOrDefault(c => c.CanSort);
                if (currentOrdered != null)
                    DefaultOrderIndex = Columns.IndexOf(currentOrdered);
                else
                    DefaultOrderIndex = 0;
            }

            Initialize();
        }

        private static void OnColumnsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = (DataGrid)obj;

            var oldList = args.OldValue as INotifyCollectionChanged;
            var newList = args.NewValue as INotifyCollectionChanged;

            if (oldList != null)
                oldList.CollectionChanged -= dataGrid.OnColumnsChanged;
            if (newList != null)
                newList.CollectionChanged += dataGrid.OnColumnsChanged;

            dataGrid.OnColumnsChanged(null, null);
        }

        private void OnColumnsChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (_initialized)
            {
                RunWithSelectedItemRestore(() =>
                {
                    UpdateColumnHeaders();

                    var itemsSource = _listControl.ItemsSource;
                    _listControl.ItemsSource = null;
                    _listControl.ItemsSource = itemsSource;
                });
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
                ChangeItemSelection(item, false);

            foreach (var item in e.AddedItems)
                ChangeItemSelection(item, true);

            var copy = SelectionChanged;
            if (copy != null)
                copy(this, e);
        }

        private void OnOrderChanged(object sender, DataGridColumnEventArgs e)
        {
            UpdateItemsOrder();

            var copy = OrderChanged;
            if (copy != null)
                copy(this, e);
        }

        private void ChangeItemSelection(object item, bool isSelected)
        {
            var listBoxItem = _listControl.GetListBoxItemFromItem(item);
            if (listBoxItem != null && listBoxItem.Content != null)
                ((DataGridRow)listBoxItem.Content).IsSelected = isSelected;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Initialize);
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
            {
                if (_initialSelectedItem != null)
                {
                    _listControl.SelectedItem = _initialSelectedItem;
                    _initialSelectedItem = null;
                }
            });
        }

        private void OnPrepareContainerForItem(object sender, PrepareContainerForItemEventArgs args)
        {
            var item = (ListBoxItem)args.Element;
            item.Padding = new Thickness(); // remove padding (use CellMargin instead)
            item.Content = new DataGridRow(this, args.Item);
            item.ContentTemplate = null;
            item.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            item.VerticalContentAlignment = VerticalAlignment.Stretch;
            item.Tapped += OnTapped;
        }

        private void OnTapped(object sender, TappedRoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var copy = Navigate;
            if (copy != null)
                copy(this, new NavigationListEventArgs(element.DataContext));
        }

        private void Initialize()
        {
            if (!_initialized)
            {
                if (_titleRowControl == null)
                    return;

                _initialized = true;

                UpdateColumnHeaders();
                if (_listControl.ItemsSource == null)
                    UpdateItemsSource();

                UpdateItemsOrder();
            }
        }

        private void UpdateColumnHeaders()
        {
            var columnIndex = 0;
            var hasStar = false;

            // Deregister old events
            foreach (var c in _titleRowControl.Children)
                c.Tapped -= OnColumnTapped;

            _titleRowControl.Children.Clear();
            _titleRowControl.ColumnDefinitions.Clear();

            foreach (var column in Columns)
            {
                // Create header element
                var title = new ContentPresenter();
                title.Content = column;
                title.ContentTemplate = HeaderTemplate;
                title.Tapped += OnColumnTapped;

                Grid.SetColumn(title, columnIndex++);
                _titleRowControl.Children.Add(title);

                // Create grid column definition
                var columnDefinition = column.CreateGridColumnDefinition();
                hasStar = hasStar || columnDefinition.Width.IsStar;
                _titleRowControl.ColumnDefinitions.Add(columnDefinition);
            }

            if (!hasStar)
                _titleRowControl.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Update selected column
            if (SelectedColumn == null || !Columns.Contains(SelectedColumn))
            {
                if (Columns.Count > DefaultOrderIndex)
                    SelectColumn(Columns[DefaultOrderIndex]);
                else if (Columns.Any(c => c.CanSort))
                    SelectColumn(Columns.First(c => c.CanSort));
                else
                    SelectedColumn = null;
            }
        }

        private void OnColumnTapped(object sender, TappedRoutedEventArgs e)
        {
            var column = (DataGridColumnBase)((ContentPresenter)sender).Content;
            if (column != null && column.CanSort)
                SelectColumn(column);
        }

        private void UpdateItemsOrder()
        {
            if (Items != null)
            {
                RunWithSelectedItemRestore(() =>
                {
                    if (SelectedColumn != null)
                    {
                        Items.IsTracking = false;
                        Items.Order = new Func<object, object>(o => PropertyPathHelper.Evaluate(o, SelectedColumn.OrderPropertyPath));
                        Items.Ascending = SelectedColumn.IsAscending;
                        Items.IsTracking = true;
                    }
                });
            }
        }

        private void RunWithSelectedItemRestore(Action action)
        {
            if (SelectionMode == SelectionMode.Single)
            {
                var previouslySelectedItem = SelectedItem;
                action();
                SelectedItem = previouslySelectedItem;
            }
            else
            {
                var previouslySelectedItems = SelectedItems.ToList();
                action();
                var currentlySelectedItems = SelectedItems.ToList();
                foreach (var item in previouslySelectedItems.Where(i => !currentlySelectedItems.Contains(i)))
                    SelectedItems.Add(item);
            }
        }

        private void UpdateItemsSource()
        {
            if (_listControl != null)
            {
                if (ItemsSource != null && !(ItemsSource is IObservableCollectionView))
                {
                    // TODO: Call dispose on ObservableView (currently using weak events)

                    // Wrap original items source with ObservableView for sorting and filtering
                    var types = ItemsSource.GetType().GenericTypeArguments;
                    var observableView = typeof(ObservableCollectionView<>).CreateGenericObject(types.Any() ?
                        types[0] : ItemsSource.GetType().GetElementType(), ItemsSource);

                    _listControl.ItemsSource = observableView;
                }
                else
                    _listControl.ItemsSource = ItemsSource;

                if (_initialFilter != null)
                {
                    Items.Filter = _initialFilter;
                    _initialFilter = null; 
                }
            }
        }
    }
}

#endif