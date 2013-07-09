using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Collections;
using MyToolkit.MVVM;
using MyToolkit.Utilities;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace MyToolkit.Controls
{
	[ContentProperty(Name = "Items")]
	public sealed class Pivot : Control
	{
		public Pivot()
		{
			DefaultStyleKey = typeof(Pivot);
		}

		private ExtendedListBox list;
		private ContentControl content;

		private int initialIndex = 0;
		private object initialItem;

		protected async override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			list = (ExtendedListBox)GetTemplateChild("List");
			content = (ContentControl)GetTemplateChild("Content");

			list.ItemsSource = Items;
			list.SelectionChanged += OnSelectionChanged;
			if (initialItem != null)
				list.SelectedItem = initialItem;
			else
				list.SelectedIndex = initialIndex;

			if (PreloadPivots)
			{
				foreach (var item in Items)
				{
					content.Content = item.Content;
					await Task.Yield(); // TODO find better solution (evtl. hidden content presenter?)
				}
				content.Content = ((PivotItem)list.SelectedItem).Content;
			}
		}

		public event SelectionChangedEventHandler SelectionChanged;

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
		{
			var item = (PivotItem)list.SelectedItem;
			content.Content = item.Content;

			SelectedIndex = list.SelectedIndex;
			SelectedItem = list.SelectedItem;

			var copy = SelectionChanged;
			if (copy != null)
				copy(sender, args);
		}

		public static readonly DependencyProperty PreloadPivotsProperty =
			DependencyProperty.Register("PreloadPivots", typeof (bool), typeof (Pivot), new PropertyMetadata(default(bool)));

		public bool PreloadPivots
		{
			get { return (bool) GetValue(PreloadPivotsProperty); }
			set { SetValue(PreloadPivotsProperty, value); }
		}

		private readonly ObservableCollection<PivotItem> items = new ObservableCollection<PivotItem>();
		public ObservableCollection<PivotItem> Items { get { return items; } }

		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof (DataTemplate), typeof (Pivot), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(object), typeof(Pivot), new PropertyMetadata(default(object), 
				(o, args) =>
					{
						if (((Pivot)o).list != null)
							((Pivot)o).list.SelectedItem = args.NewValue;
						else
							((Pivot)o).initialItem = args.NewValue;
					}));

		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Pivot), new PropertyMetadata(default(int),
				(o, args) =>
				{
					if (((Pivot)o).list != null)
						((Pivot)o).list.SelectedIndex = (int)args.NewValue;
					else
						((Pivot)o).initialIndex = (int)args.NewValue;
				}));

		public int SelectedIndex
		{
			get { return (int) GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
	}
}
