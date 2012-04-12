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

namespace MyToolkit.Controls
{
	public sealed class DataGrid : Control
	{
		public DataGrid()
		{
			DefaultStyleKey = typeof(DataGrid);
		}

		private Grid titleRowControl;
		private NavigationList listControl;

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			titleRowControl = (Grid)GetTemplateChild("titles");
			listControl = (NavigationList)GetTemplateChild("list");
			listControl.PrepareContainerForItem += OnPrepareContainerForItem;

			BuildUp();
		}

		private void OnPrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
		{
			var item = (ListBoxItem)e.Element;

			var grid = new Grid();
			grid.Margin = new Thickness(10, 0, 0, 5);

			var x = 0;
			foreach (var c in Columns)
			{
				var ctrl = c.GenerateElement(e.Item);

				grid.ColumnDefinitions.Add(c.CreateGridColumnDefinition());
				grid.Children.Add(ctrl);

				Grid.SetColumn(ctrl, x++);
			}

			item.Content = grid;
			item.ContentTemplate = null;
			item.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			item.VerticalContentAlignment = VerticalAlignment.Stretch;
		}

		public event EventHandler<NavigationListEventArgs> Navigated;

		private void OnNavigated(object sender, NavigationListEventArgs e)
		{
			var copy = Navigated;
			if (copy != null)
				copy(this, e);
		}

		public void BuildUp()
		{
			var x = 0;
			titleRowControl.ColumnDefinitions.Clear();
			foreach (var c in Columns)
			{
				var title = new ContentControl 
				{ 
					ContentTemplate = HeaderTemplate, 
					Content = c, 
					VerticalContentAlignment = VerticalAlignment.Stretch, 
					HorizontalContentAlignment = HorizontalAlignment.Stretch, 
				};
				title.Tapped += OnTapped; 
				
				Grid.SetColumn(title, x++);
				titleRowControl.Children.Add(title);
				titleRowControl.ColumnDefinitions.Add(c.CreateGridColumnDefinition());
			}

			UpdateItemsSource();

			SelectColumn(Columns[DefaultOrderIndex]);
		}

		public void SelectColumn(DataGridColumn column)
		{
			var old = sortedColumn;
			if (old != null)
				old.IsSelected = false;

			sortedColumn = column;
			sortedColumn.IsSelected = true;
			sortedColumn.IsAscending = old == sortedColumn ? !sortedColumn.IsAscending : sortedColumn.IsAscendingDefault;

			UpdateOrder();
		}

		private DataGridColumn sortedColumn; 
		private void OnTapped(object sender, TappedRoutedEventArgs e)
		{
			SelectColumn((DataGridColumn)((ContentControl)sender).Content);		
		}

		public IExtendedObservableCollection Items
		{
			get { return listControl == null ? null : (IExtendedObservableCollection)listControl.ItemsSource; }
		}

		private void UpdateOrder()
		{
			if (Items != null)
			{
				Items.IsTracking = false;
				Items.Order = new Func<object, object>(o => PropertyPathHelper.Evaluate(o, sortedColumn.Binding.Path));
				Items.Ascending = sortedColumn.IsAscending;
				Items.IsTracking = true;
			}
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
				try
				{
					var l = ItemsSource; 
					if (l != null && !(l is IExtendedObservableCollection))
					{
						var type = ItemsSource.GetType().GenericTypeArguments[0];
						if (ItemsSource is INotifyCollectionChanged) // is ObservableCollection => wrap with ExtendedObservableCollection
							l = typeof(ExtendedObservableCollection<>).CreateGenericObject(type, ItemsSource);
						else // is ObservableCollection => wrap with ExtendedObservableCollection and ObservableCollection
							l = typeof(ExtendedObservableCollection<>).CreateGenericObject(type, typeof(ObservableCollection<>).CreateGenericObject(type, ItemsSource));
					}
					listControl.ItemsSource = l;
				}
				catch { } // TODO: remove workaround ()
			}
		}

		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}

		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(DataGrid), new PropertyMetadata(default(DataTemplate)));
	}

	public class DataGridTextColumn : DataGridColumn
	{
		private double? fontSize;
		[DefaultValue(double.NaN)]
		public double FontSize
		{
			get { return fontSize ?? Double.NaN; }
			set { fontSize = value; }
		}

		private FontStyle? fontStyle;
		public FontStyle FontStyle
		{
			get { return fontStyle ?? FontStyle.Normal; }
			set { fontStyle = value; }
		}

		private Brush foreground;
		public Brush Foreground
		{
			get { return foreground; }
			set { foreground = value; }
		}

		private Style style;
		public Style Style
		{
			get { return style; }
			set { style = value; }
		}

		public override FrameworkElement GenerateElement(object dataItem)
		{
			var block = new TextBlock();
			block.VerticalAlignment = VerticalAlignment.Center;

			if (style != null)
				block.Style = style;
			
			if (fontSize.HasValue)
				block.FontSize = fontSize.Value;

			if (fontStyle.HasValue)
				block.FontStyle = fontStyle.Value;

			if (foreground != null)
				block.Foreground = foreground;

			if (Binding != null)
				block.SetBinding(TextBlock.TextProperty, Binding); 

			return block;
		}
	}

	public abstract class DataGridColumn : DependencyObject
	{
		public abstract FrameworkElement GenerateElement(object dataItem);

		internal ColumnDefinition CreateGridColumnDefinition()
		{
			return new ColumnDefinition { Width = Width == 0.0 ? GridLengthHelper.Auto : GridLengthHelper.FromPixels(Width) };
		}

		public bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			internal set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyProperty.Register("IsSelected", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(default(bool)));


		public bool IsAscending
		{
			get { return (bool)GetValue(IsAscendingProperty); }
			internal set { SetValue(IsAscendingProperty, value); }
		}
		public static readonly DependencyProperty IsAscendingProperty =
			DependencyProperty.Register("IsAscending", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(default(bool)));





		public bool IsAscendingDefault
		{
			get { return (bool)GetValue(IsAscendingDefaultProperty); }
			set { SetValue(IsAscendingDefaultProperty, value); }
		}
		public static readonly DependencyProperty IsAscendingDefaultProperty =
			DependencyProperty.Register("IsAscendingDefault", typeof(bool), typeof(DataGridColumn), new PropertyMetadata(true));


		public string Header
		{
			get { return (string)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof(string), typeof(DataGridColumn), new PropertyMetadata(default(string)));

		private Binding binding; 
		public Binding Binding
		{
			get { return binding; }
			set { binding = value; }
			//get { return (Binding)GetValue(BindingProperty); }
			//set { SetValue(BindingProperty, value); }
		}
		//public static readonly DependencyProperty BindingProperty =
		//	DependencyProperty.Register("Binding", typeof(Binding), typeof(DataGridColumn), new PropertyMetadata(default(Binding)));


		public double Width
		{
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.Register("Width", typeof(double), typeof(DataGridColumn), new PropertyMetadata(default(double)));
	}
}
