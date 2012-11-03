using System.Collections.ObjectModel;
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
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			list = (ExtendedListBox)GetTemplateChild("List");
			content = (ContentControl)GetTemplateChild("Content");

			//content.Transitions = new TransitionCollection();
			//content.Transitions.Add(new EntranceThemeTransition());

			list.ItemsSource = Items;
			list.SelectionChanged += OnSelectionChanged;
			list.SelectedIndex = 0; 
		}

		public event SelectionChangedEventHandler SelectionChanged;

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
		{
			var item = (PivotItem)list.SelectedItem;
			//content.ContentTemplate = item.Template;
			//content.Content = this.FindParentDataContext();
			content.Content = item.Content;

			SelectedIndex = list.SelectedIndex;
			SelectedItem = list.SelectedItem;

			var copy = SelectionChanged;
			if (copy != null)
				copy(sender, args);
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
			DependencyProperty.Register("SelectedItem", typeof(object), typeof(Pivot), new PropertyMetadata(default(object), (o, args) => ((Pivot)o).list.SelectedItem = args.NewValue));

		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}


		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Pivot), new PropertyMetadata(default(int), (o, args) => ((Pivot)o).list.SelectedIndex = (int)args.NewValue));

		public int SelectedIndex
		{
			get { return (int) GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}	
	}
}
