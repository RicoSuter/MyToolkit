//-----------------------------------------------------------------------
// <copyright file="DataGrid.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Core;
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
        private DataGridColumn _selectdColumn;
        private MtListBox _listControl;
        private bool _initialized = false;
        private object _initialSelectedItem;

        public DataGrid()
        {
            DefaultStyleKey = typeof(DataGrid);

            Columns = new ObservableCollection<DataGridColumn>(); // Needed so that columns can be defined in XAML
            if (!Designer.IsInDesignMode)
                Loaded += OnLoaded;
        }

        /// <summary>
        /// Occurs when the selected item (row) has changed. 
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the user clicked on an item and wants to navigate to its detail page. 
        /// </summary>
        public event EventHandler<NavigationListEventArgs> Navigate;

        #region Dependency properties

        public static readonly DependencyProperty HeaderBackgroundProperty = 
            DependencyProperty.Register("HeaderBackground", typeof (Brush), typeof (DataGrid), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets or sets the header background. 
        /// </summary>
        public Brush HeaderBackground
        {
            get { return (Brush) GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(DataGrid), new PropertyMetadata(null, OnSelectedItemChanged));

        /// <summary>
        /// Gets or sets the selected item. 
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(DataGrid), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Gets or sets the collection to show in the <see cref="DataGrid"/>. 
        /// </summary>
        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty DefaultOrderIndexProperty =
            DependencyProperty.Register("DefaultOrderIndex", typeof(int), typeof(DataGrid), new PropertyMetadata(-1));

        /// <summary>
        /// Gets or sets the index of the column which is initially ordered. 
        /// </summary>
        public int DefaultOrderIndex
        {
            get { return (int)GetValue(DefaultOrderIndexProperty); }
            set { SetValue(DefaultOrderIndexProperty, value); }
        }

        public static readonly DependencyProperty RowStyleProperty =
            DependencyProperty.Register("RowStyle", typeof(Style), typeof(DataGrid), new PropertyMetadata(default(Style)));

        /// <summary>
        /// Used to change the row style, the ItemContainerStyle of the internal ListBox; use ListBoxItem as style target type. 
        /// </summary>
        public Style RowStyle
        {
            get { return (Style)GetValue(RowStyleProperty); }
            set { SetValue(RowStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemDetailsTemplateProperty =
            DependencyProperty.Register("ItemDetailsTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Gets or sets the data template for item details (shown when an item is selected). When null then no details are shown. 
        /// </summary>
        public DataTemplate ItemDetailsTemplate
        {
            get { return (DataTemplate)GetValue(ItemDetailsTemplateProperty); }
            set { SetValue(ItemDetailsTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Gets or sets the header data template (styling of column container).
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Gets or sets the cell data template (currently not working). 
        /// </summary>
        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        #endregion

        /// <summary>
        /// Gets a value indicating whether the item details are shown. 
        /// </summary>
        public bool ShowItemDetails
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the currently displayed items. 
        /// </summary>
        public IObservableCollectionView Items
        {
            get { return _listControl == null ? null : _listControl.ItemsSource as IObservableCollectionView; }
        }

        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns",
              typeof(ObservableCollection<DataGridColumn>), typeof(DataGrid),
              new PropertyMetadata(null, OnColumnsPropertyChanged));

        /// <summary>
        /// Gets the column description of the <see cref="DataGrid"/>. 
        /// </summary>
        public ObservableCollection<DataGridColumn> Columns
        {
            get { return (ObservableCollection<DataGridColumn>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Gets the selected column. 
        /// </summary>
        public DataGridColumn SelectedColumn
        {
            get { return _selectdColumn; }
        }

        /// <summary>
        /// Selects a column for ordering. 
        /// If the column is not selected the the default ordering is used (IsAscendingDefault property). 
        /// If the column is already selected then the ordering direction will be inverted. 
        /// </summary>
        /// <param name="column">The column. </param>
        /// <returns>Returns true if column could be changed. </returns>
        public bool SelectColumn(DataGridColumn column)
        {
            return SelectColumn(column, column == _selectdColumn ? !column.IsAscending : column.IsAscendingDefault);
        }

        /// <summary>
        /// Selects a column for ordering. 
        /// </summary>
        /// <param name="column">The column. </param>
        /// <param name="ascending">The value indicating whether to sort the column ascending; otherwise descending. </param>
        /// <returns>Returns true if column could be changed. </returns>
        public bool SelectColumn(DataGridColumn column, bool ascending)
        {
            if (column.CanSort)
            {
                var oldColumn = _selectdColumn;
                if (oldColumn != null)
                    oldColumn.IsSelected = false;

                _selectdColumn = column;
                _selectdColumn.IsSelected = true;
                _selectdColumn.IsAscending = ascending;

                UpdateOrder();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the filter on the items source. 
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="filter"></param>
        public void SetFilter<TItem>(Func<TItem, bool> filter)
        {
            Items.Filter = filter; 
        }

        /// <summary>
        /// Removes the current filter. 
        /// </summary>
        public void RemoveFilter()
        {
            Items.Filter = null; 
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _titleRowControl = (Grid)GetTemplateChild("ColumnHeaders");

            _listControl = (MtListBox)GetTemplateChild("Rows");
            _listControl.PrepareContainerForItem += OnPrepareContainerForItem;
            _listControl.SelectionChanged += OnSelectionChanged;

            _initialSelectedItem = SelectedItem;

            if (DefaultOrderIndex == -1)
            {
                var currentOrdered = Columns.FirstOrDefault(c => c.CanSort);
                if (currentOrdered != null)
                    DefaultOrderIndex = Columns.IndexOf(currentOrdered);
                else
                    DefaultOrderIndex = 0;
            }

            if (!_initialized)
                Initialize();
        }

        private static void OnItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataGrid)obj).UpdateItemsSource();
        }

        private static void OnColumnsPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = (DataGrid)obj;

            var oldList = args.OldValue as ObservableCollection<DataGridColumn>;
            var newList = args.NewValue as ObservableCollection<DataGridColumn>;

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
                var selectedItem = SelectedItem;
                UpdateColumnHeaders();

                // update rows
                var itemsSource = _listControl.ItemsSource;
                _listControl.ItemsSource = null;
                _listControl.ItemsSource = itemsSource;

                SelectedItem = selectedItem;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lastItem = e.RemovedItems.Count > 0 ? _listControl.GetListBoxItemFromItem(e.RemovedItems[0]) : null;
            var newItem = e.AddedItems.Count > 0 ? _listControl.GetListBoxItemFromItem(e.AddedItems[0]) : null;

            if (lastItem != null && lastItem.Content != null)
                ((DataGridRow)lastItem.Content).IsSelected = false;
            if (newItem != null && newItem.Content != null)
                ((DataGridRow)newItem.Content).IsSelected = true;

            SelectedItem = _listControl.SelectedItem;

            var copy = SelectionChanged;
            if (copy != null)
                copy(sender, e);
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

        private static void OnSelectedItemChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = (DataGrid)obj;
            if (dataGrid._listControl != null)
                dataGrid._listControl.SelectedItem = args.NewValue;
        }

        private void OnPrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
        {
            var row = new DataGridRow(this, e.Item);
            row.IsSelected = e.Item == SelectedItem;

            var item = (ListBoxItem)e.Element;
            item.Content = row;
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
            if (_titleRowControl == null)
                return;

            _initialized = true;

            UpdateColumnHeaders();
            if (_listControl.ItemsSource == null)
                UpdateItemsSource();
        }

        private void UpdateColumnHeaders()
        {
            var columnIndex = 0;
            var hasStar = false;

            // Unregister old events
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
            if (_selectdColumn == null || !Columns.Contains(_selectdColumn))
            {
                if (Columns.Count > DefaultOrderIndex)
                    SelectColumn(Columns[DefaultOrderIndex]);
                else if (Columns.Any(c => c.CanSort))
                    SelectColumn(Columns.First(c => c.CanSort));
                else
                    _selectdColumn = null;
            }
        }

        private void OnColumnTapped(object sender, TappedRoutedEventArgs e)
        {
            var column = (DataGridColumn)((ContentPresenter)sender).Content;
            if (column != null && column.CanSort)
                SelectColumn(column);
        }

        private void UpdateOrder()
        {
            if (Items != null)
            {
                var selectedItem = SelectedItem;

                Items.IsTracking = false;
                Items.Order = new Func<object, object>(o => PropertyPathHelper.Evaluate(o, _selectdColumn.OrderPropertyPath));
                Items.Ascending = _selectdColumn.IsAscending;
                Items.IsTracking = true;

                SelectedItem = selectedItem;
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
            }
        }
    }
}
