using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.UI.Xaml;

namespace MyToolkit.UI.Popups
{
	public partial class ListPickerBox
	{
		#region Static methods

		public static async Task<ListPickerBox> ShowAsync(IList items, IList selectedItems, bool minOneSelected = true)
		{
			var popup = new ListPickerBox(items, selectedItems, minOneSelected);
			await PopupHelper.ShowDialogAsync(popup, false, false);
			return popup; 
		}

		#endregion

		private bool minOneSelected; 
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

		protected ListPickerBox(IList items, IList selectedItems, bool minOneSelected)
		{
			InitializeComponent();

			this.minOneSelected = minOneSelected; 
			AllItems = items; 
			OriginalSelectedItems = selectedItems; 
			Loaded += OnLoaded;
		}


		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			Items = AllItems.OfType<object>().
					Select(i => new ListPickerBoxItem { Item = i, IsChecked = OriginalSelectedItems.Contains(i) }).
					ToList();

			UpdateSelectedItems();
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
		}

		private void CheckBox_Click_1(object sender, RoutedEventArgs e)
		{
			UpdateSelectedItems();
			if (minOneSelected)
				acceptButton.IsEnabled = SelectedItems.Count > 0; 
		}
	}

	public class ListPickerBoxItem
	{
		public bool IsChecked { get; set; }
		public object Item { get; set; }
	}
}
