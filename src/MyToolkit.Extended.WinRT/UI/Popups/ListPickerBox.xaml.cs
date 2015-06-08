using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Model;
using MyToolkit.Resources;
using MyToolkit.Utilities;
using Windows.UI.Xaml;

namespace MyToolkit.UI.Popups
{
	public partial class ListPickerBox
	{
		#region Static methods

		public static async Task<ListPickerBox> ShowAsync(string header, IList items, IList selectedItems, 
			bool minOneSelected = true, bool showSelectAllButton = false)
		{
			var popup = new ListPickerBox(header, items ?? new List<object>(), selectedItems ?? new List<object>(), minOneSelected, showSelectAllButton);
			await PopupHelper.ShowVerticalDialogAsync(popup, false);
			return popup; 
		}

		#endregion

		private readonly bool _minOneSelected; 

		public List<ListPickerBoxItem> Items { get; private set; }

		public IList SelectedItems { get; private set; }
		public IList AllItems { get; private set; }
		public IList OriginalSelectedItems { get; private set; }

		public bool Canceled { get; private set; }

		public bool HasSelectionChanged
		{
			get
			{
				return !OriginalSelectedItems.OfType<object>().ToList().
					IsCopyOf(SelectedItems.OfType<object>().ToList());
			}
		}

		protected ListPickerBox(string header, IList items, IList selectedItems, bool minOneSelected, bool showSelectAllButton)
		{
			InitializeComponent();

			this.header.Text = header; 
			_minOneSelected = minOneSelected;
			SelectAllButton.Visibility = showSelectAllButton ? Visibility.Visible : Visibility.Collapsed;

			AllItems = items; 
			OriginalSelectedItems = selectedItems; 
			Loaded += OnLoaded;
		}

		private void UpdateSelectAllButton()
		{
			if (Items.All(i => i.IsChecked))
				SelectAllButton.Content = Strings.UncheckAllButton;
			else
				SelectAllButton.Content = Strings.CheckAllButton;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			Items = AllItems.OfType<object>().
					Select(i => new ListPickerBoxItem { Item = i, IsChecked = OriginalSelectedItems.Contains(i) }).
					ToList();

			UpdateSelectedItems();
			UpdateSelectAllButton();

			list.ItemsSource = Items;
		}

		private void OnCancel(object sender, RoutedEventArgs routedEventArgs)
		{
			Canceled = true; 
			Close();
		}

		private void OnAccept(object sender, RoutedEventArgs e)
		{
			UpdateSelectedItems();
			Close();
		}

		private void UpdateSelectedItems()
		{
			SelectedItems = Items.Where(i => i.IsChecked).
				Select(i => i.Item).ToList();
		
			if (_minOneSelected)
				acceptButton.IsEnabled = SelectedItems.Count > 0;
		}

		private void CheckBox_Click_1(object sender, RoutedEventArgs e)
		{
			UpdateSelectedItems();
			UpdateSelectAllButton();
		}

		private void OnSelectAll(object sender, RoutedEventArgs e)
		{
			if (Items.All(i => i.IsChecked))
			{
				foreach (var item in Items)
					item.IsChecked = false;
			}
			else
			{
				foreach (var item in Items)
					item.IsChecked = true;
			}

			UpdateSelectedItems();
			UpdateSelectAllButton();
		}
	}

	public class ListPickerBoxItem : ObservableObject
	{
		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set { Set(ref _isChecked, value); }
		}

		public object Item { get; set; }
	}
}
