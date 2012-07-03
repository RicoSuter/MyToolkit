using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyToolkit.UI.UIExtensionMethods;
using Windows.UI.Xaml;
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
			content.ContentTemplate = item.Template;
			content.Content = this.FindParentDataContext();
		}

		//public static readonly DependencyProperty ItemsProperty =
		//	DependencyProperty.Register("Items", typeof(ObservableCollection<PivotItem>), typeof(Pivot), new PropertyMetadata(default(PivotItem)));

		//public ObservableCollection<PivotItem> Items
		//{
		//	get { return (ObservableCollection<PivotItem>)GetValue(ItemsProperty); }
		//	set { SetValue(ItemsProperty, value); }
		//}	
		ObservableCollection<PivotItem> items = new ObservableCollection<PivotItem>();
		public ObservableCollection<PivotItem> Items
		{
			get
			{
				return items;
			}
		}
	}

	[ContentProperty(Name = "Template")]
	public class PivotItem : DependencyObject
	{
		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (string), typeof (PivotItem), new PropertyMetadata(default(string)));

		public string Header
		{
			get { return (string) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty TemplateProperty =
			DependencyProperty.Register("Template", typeof (DataTemplate), typeof (PivotItem), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate Template
		{
			get { return (DataTemplate) GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
	}
}
