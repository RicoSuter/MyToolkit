using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MyToolkit.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using MyToolkit.Utilities;
using System.Threading.Tasks; 

namespace MyToolkit.Controls
{
	public sealed class DataGrid : Control
	{
		private bool loaded = false; 
		public DataGrid()
		{
			DefaultStyleKey = typeof(DataGrid);
			Loaded += OnLoaded; 
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (loaded)
				return;

			loaded = true; 
			Dispatcher.Invoke(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
			{
				BuildUp();
			}, this, null);
		}

		private Grid titleRowControl;
		private NavigationList listControl;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			titleRowControl = (Grid)GetTemplateChild("titles");

			listControl = (NavigationList)GetTemplateChild("list");
			listControl.PrepareContainerForItem += OnPrepareContainerForItem;
			listControl.SelectionChanged += OnSelectionChanged;
			listControl.Navigate += OnNavigate; 

			var currentOrdered = Columns.FirstOrDefault(c => c.CanSort); 
			if (currentOrdered != null)
				DefaultOrderIndex = Columns.IndexOf(currentOrdered);
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var lastItem = e.RemovedItems.Count > 0 ? listControl.GetListBoxItemFromItem(e.RemovedItems[0]) : null;
			var newItem = e.AddedItems.Count > 0 ? listControl.GetListBoxItemFromItem(e.AddedItems[0]) : null;

			if (lastItem != null)
				((DataGridRow)lastItem.Content).IsSelected = false;
			if (newItem != null)
				((DataGridRow)newItem.Content).IsSelected = true;

			if (SelectedItem != listControl.SelectedItem)
				SelectedItem = listControl.SelectedItem; 
		}

		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(object), typeof(DataGrid), new PropertyMetadata(null));

		private void OnPrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
		{
			var item = (ListBoxItem)e.Element;
			item.Content = new DataGridRow(this, e.Item);
			item.ContentTemplate = null; 
			item.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			item.VerticalContentAlignment = VerticalAlignment.Stretch;
		}

		public event EventHandler<NavigationListEventArgs> Navigate;

		private void OnNavigate(object sender, NavigationListEventArgs e)
		{
			var copy = Navigate;
			if (copy != null)
			{
				copy(this, e);
				listControl.SelectedItem = null; // TODO add dependency property for setting
			}
		}

		private void BuildUp()
		{
			var x = 0;
			var hasStar = false; 

			titleRowControl.ColumnDefinitions.Clear();
			foreach (var c in Columns)
			{
				var title = new ContentPresenter();
				title.Content = c;
				title.ContentTemplate = HeaderTemplate;

				title.Tapped += OnTapped; 
				
				Grid.SetColumn(title, x++);
				titleRowControl.Children.Add(title);

				var def = c.CreateGridColumnDefinition();
				hasStar = hasStar || def.Width.IsStar;
				titleRowControl.ColumnDefinitions.Add(def);
			}

			if (!hasStar)
				titleRowControl.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			UpdateItemsSource();
			SelectColumn(Columns[DefaultOrderIndex]);
		}

		public void SelectColumn(DataGridColumn column)
		{
			var old = sortedColumn;
			if (old != null)
				old.IsSelected = false;

			sortedColumn = column;
			sortedColumn.IsSelected = sortedColumn.CanSort;
			sortedColumn.IsAscending = old == sortedColumn ? !sortedColumn.IsAscending : sortedColumn.IsAscendingDefault;

			UpdateOrder();
		}

		private DataGridColumn sortedColumn; 
		private void OnTapped(object sender, TappedRoutedEventArgs e)
		{
			var column = (DataGridColumn)((ContentPresenter)sender).Content; 
			if (column.CanSort)
				SelectColumn(column);		
		}

		public IExtendedObservableCollection Items
		{
			get { return listControl == null ? null : listControl.ItemsSource as IExtendedObservableCollection; }
		}

		private void UpdateOrder()
		{
			if (Items != null)
			{
				Items.IsTracking = false;
				Items.Order = new Func<object, object>(o => PropertyPathHelper.Evaluate(o, sortedColumn.OrderPropertyPath));
				Items.Ascending = sortedColumn.IsAscending;
				Items.IsTracking = true;
			}
		}

		public bool ShowItemDetails
		{
			get { return true; }
		}

		ObservableCollection<DataGridColumn> columns = new ObservableCollection<DataGridColumn>();
		public ObservableCollection<DataGridColumn> Columns
		{
			get
			{
				return columns;
			}
		}

		public object ItemsSource
		{
			get { return (object)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(object), typeof(DataGrid), new PropertyMetadata(null, ItemsSourceChanged));



		public int DefaultOrderIndex
		{
			get { return (int)GetValue(DefaultOrderIndexProperty); }
			set { SetValue(DefaultOrderIndexProperty, value); }
		}

		public static readonly DependencyProperty DefaultOrderIndexProperty =
			DependencyProperty.Register("DefaultOrderIndex", typeof(int), typeof(DataGrid), new PropertyMetadata(0));



		private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (DataGrid)d;
			ctrl.UpdateItemsSource();
		}

		private void UpdateItemsSource()
		{
			if (listControl != null)
			{
				if (ItemsSource != null && !(ItemsSource is IExtendedObservableCollection))
				{
					var type = ItemsSource.GetType().GenericTypeArguments[0];
					var newList = typeof(ExtendedObservableCollection<>).CreateGenericObject(type, ItemsSource);
					listControl.ItemsSource = newList;
				}
				else
					listControl.ItemsSource = ItemsSource;
			}
		}


		public DataTemplate ItemDetailsTemplate
		{
			get { return (DataTemplate)GetValue(ItemDetailsTemplateProperty); }
			set { SetValue(ItemDetailsTemplateProperty, value); }
		}

		public static readonly DependencyProperty ItemDetailsTemplateProperty =
			DependencyProperty.Register("ItemDetailsTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));



		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));



		public DataTemplate CellTemplate
		{
			get { return (DataTemplate)GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}

		public static readonly DependencyProperty CellTemplateProperty =
			DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));
	}
}
