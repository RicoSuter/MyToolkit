using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyToolkit.Data;
using MyToolkit.Model;
using MyToolkit.MVVM;
using MyToolkit.Resources;
using MyToolkit.Utilities; 

namespace MyToolkit.UI.Popups
{
	public partial class ListPickerBox
	{
		#region Static methods

		public static Task<ListPickerBox> ShowAsync(IList items, IList selectedItems, string title = null, bool minOneSelected = true, bool showSelectAllButton = false)
		{
			var source = new TaskCompletionSource<ListPickerBox>();
			Show(items, selectedItems, title, minOneSelected, showSelectAllButton, source.SetResult);
			return source.Task;
		}

		public static Task<ListPickerBox> ShowSingleAsync(IList items, object selectedItem, string title = null)
		{
			var source = new TaskCompletionSource<ListPickerBox>();
			ShowSingle(items, selectedItem, title, source.SetResult);
			return source.Task;
		}

		public static void Show(IList items, IList selectedItems, string title, bool minOneSelected, bool showSelectAllButton, Action<ListPickerBox> completed)
		{
			var popup = new ListPickerBox(items, selectedItems, false, title, minOneSelected, showSelectAllButton);
			Show(popup, true, true, x => completed((ListPickerBox)x));
		}

		public static void ShowSingle(IList items, object selectedItem, string title, Action<ListPickerBox> completed)
		{
			var popup = new ListPickerBox(items, selectedItem != null ? new List<object> { selectedItem } : null, true, title);
			Show(popup, true, true, x => completed((ListPickerBox)x));
		}

		#endregion

		public List<ListPickerBoxItem> Items { get; private set; }

		public IList SelectedItems { get; private set; }
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

		private bool minOneSelected;
		protected ListPickerBox(IList items, IList selectedItems, bool singleSelection, string _title, bool minOneSelected = true, bool showSelectAllButton = false)
		{
			InitializeComponent();
			this.minOneSelected = minOneSelected; 

			if (string.IsNullOrEmpty(_title))
				title.Visibility = Visibility.Collapsed;
			else
				title.Text = _title; 

			if (selectedItems == null)
				selectedItems = new List<object>();

			SelectAllButton.Visibility = showSelectAllButton ? Visibility.Visible : Visibility.Collapsed;

			list.ItemTemplate = (DataTemplate)(singleSelection ? Resources["singleTemplate"] : Resources["multipleTemplate"]);
			OriginalSelectedItems = selectedItems; 
			
			Dispatcher.BeginInvoke(delegate
			{
				Items = items.OfType<object>().
					Select(i => new ListPickerBoxItem { Item = i, IsChecked = selectedItems.Contains(i) }).
					ToList();

				UpdateSelectedItems();
				UpdateSelectAllButton();

				list.ItemsSource = Items; 
			});
		}

		public override void GoBack()
		{
			Canceled = true;
			OnAccept(null, null);
		}

		private void OnCancel(object sender, RoutedEventArgs e)
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

			if (minOneSelected)
				acceptButton.IsEnabled = SelectedItems.Count > 0;
		}

		private void UpdateSelectAllButton()
		{
			if (Items.All(i => i.IsChecked))
				SelectAllButton.Content = Strings.UncheckAllButton;
			else
				SelectAllButton.Content = Strings.CheckAllButton;
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

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			UpdateSelectedItems();
			UpdateSelectAllButton();
		}
	}

	public class ListPickerBoxItem : NotifyPropertyChanged
	{
		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set { SetProperty(ref _isChecked, value); }
		}

		public object Item { get; set; }
	}
}
