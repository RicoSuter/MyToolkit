using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MyToolkit.UI.Popups
{
	public partial class ListPickerBox
	{
		#region Static methods

		public static void Show(IList items, IList selectedItems, Action<ListPickerBox> completed)
		{
			var popup = new ListPickerBox(items, selectedItems);
			Show(popup, true, true, x => completed((ListPickerBox)x));
		}

		#endregion

		public List<ListPickerBoxItem> Items { get; private set; }
		public IList SelectedItems { get; private set; }
		public bool Canceled { get; private set; }

		protected ListPickerBox(IList items, IList selectedItems)
		{
			InitializeComponent();

			Dispatcher.BeginInvoke(delegate
			{
				Items = items.OfType<object>().
					Select(i => new ListPickerBoxItem { Item = i, IsChecked = selectedItems.Contains(i) }).
					ToList();

				UpdateSelectedItems();
				list.ItemsSource = Items; 
			});
		}

		public override void GoBack()
		{
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
		}
	}

	public class ListPickerBoxItem
	{
		public bool IsChecked { get; set; }
		public object Item { get; set; }
	}
}
