using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyToolkit.UI.UIExtensionMethods;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	[ContentProperty(Name = "Items")]
	public sealed class Pivot : Control
	{
		public Pivot()
		{
			this.DefaultStyleKey = typeof(Pivot);
		}

		private ExtendedListBox list;
		private ContentPresenter content; 
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			list = (ExtendedListBox)GetTemplateChild("List");
			content = (ContentPresenter)GetTemplateChild("Content");

			list.ItemsSource = Items;
			list.SelectionChanged += OnSelectionChanged;
			list.SelectedIndex = 0; 
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
		{
			var item = (PivotItem)list.SelectedItem;
			//content.ContentTemplate = item.Template;
			//content.Content = this.FindParentDataContext();
			content.Content = item.Content;
		}

		private readonly ObservableCollection<PivotItem> items = new ObservableCollection<PivotItem>();
		public ObservableCollection<PivotItem> Items { get { return items; } }
	}
}
