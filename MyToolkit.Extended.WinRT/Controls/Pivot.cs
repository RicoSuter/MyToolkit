//-----------------------------------------------------------------------
// <copyright file="Pivot.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// TODO: Rename to MtPivot to avoid name problems with WP pivot

namespace MyToolkit.Controls
{
	[ContentProperty(Name = "Items")]
	public sealed class Pivot : Control
	{
        private MtListBox _list;
        private Grid _grid;
        private Storyboard _story;
        private TranslateTransform _translate;

        private int _initialIndex = 0;
        private object _initialItem;
        
        public Pivot()
		{
			DefaultStyleKey = typeof(Pivot);
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
				_list.SelectedItem = _initialItem;
			else
				_list.SelectedIndex = _initialIndex;
		}

		private void AddElement(FrameworkElement element)
		{
			if (!_grid.Children.Contains(element))
				_grid.Children.Add(element);				
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			foreach (PivotItem item in args.OldItems)
			{
				if (!_items.Contains(item) && _grid.Children.Contains(item.Content))
					_grid.Children.Remove(item.Content);
			}

			foreach (PivotItem item in args.NewItems)
			{
				if (_items.Contains(item) && item.Preload)
					AddElement(item.Content);
			}
		}

		private void ShowSelectedPivot()
		{
			_translate.X = 30;
			CurrentPrivotElement.Visibility = Visibility.Visible;
			_story.Begin();
		}

		private FrameworkElement CurrentPrivotElement
		{
			get { return ((PivotItem) _list.SelectedItem).Content; }
		}

		public event SelectionChangedEventHandler SelectionChanged;

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
		{
			foreach (var item in _grid.Children)
				item.Visibility = Visibility.Collapsed;

			AddElement(CurrentPrivotElement);

			SelectedIndex = _list.SelectedIndex;
			SelectedItem = _list.SelectedItem;

			var copy = SelectionChanged;
			if (copy != null)
				copy(sender, args);

			ShowSelectedPivot();
		}

		private readonly ObservableCollection<PivotItem> _items = new ObservableCollection<PivotItem>();
		public ObservableCollection<PivotItem> Items { get { return _items; } }

		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof (DataTemplate), typeof (Pivot), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(object), typeof(Pivot), new PropertyMetadata(default(object), 
				(o, args) => {
					if (((Pivot)o)._list != null)
						((Pivot)o)._list.SelectedItem = args.NewValue;
					else
						((Pivot)o)._initialItem = args.NewValue;
				}));

		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Pivot), new PropertyMetadata(default(int),
				(o, args) => {
					if (((Pivot)o)._list != null)
						((Pivot)o)._list.SelectedIndex = (int)args.NewValue;
					else
						((Pivot)o)._initialIndex = (int)args.NewValue;
				}));

		public int SelectedIndex
		{
			get { return (int) GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
	}
}
