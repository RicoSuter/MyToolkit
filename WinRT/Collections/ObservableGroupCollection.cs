using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Collections
{
	public class ObservableGroupCollection<T> : ObservableCollection<IGroup>
	{
		public ExtendedGroup<T> AddGroup(string title, int topItemsCount = -1)
		{
			return AddGroup(title, new List<T>(), topItemsCount);
		}

		public ExtendedGroup<T> AddGroup(string title, IEnumerable<T> items, int topItemsCount = -1)
		{
			var group = new ExtendedGroup<T>(title, items, topItemsCount);
			Add(group);
			return group;
		}

		private CollectionViewSource view; 
		public CollectionViewSource View
		{
			get
			{
				if (view == null)
				{
					view = new CollectionViewSource();
					view.IsSourceGrouped = true;
					view.ItemsPath = new PropertyPath("TopItems");
					view.Source = this; 
				}
				return view; 
			}
		}
	}
}