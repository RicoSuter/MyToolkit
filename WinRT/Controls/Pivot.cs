using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

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




		public static readonly DependencyProperty HeaderTemplateProperty =
			DependencyProperty.Register("HeaderTemplate", typeof (DataTemplate), typeof (Pivot), new PropertyMetadata(default(DataTemplate)));

		public DataTemplate HeaderTemplate
		{
			get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}	
	}
}
