using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyToolkit.Collections
{
	public class ExtendedGroup<T> : Group<T>
	{
		public int TopItemsCount { get; set; }

		public ExtendedGroup(string title) : base(title)
		{
			TopItemsCount = 6;
			TopItems = new ObservableCollection<T>(this.Take(TopItemsCount));
		}

		public ExtendedGroup(string title, IEnumerable<T> items) : base(title, items)
		{
			TopItemsCount = 6;
			TopItems = new ObservableCollection<T>(this.Take(TopItemsCount));
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
			TopItems.Clear();
			foreach (var i in this.Take(TopItemsCount))
				TopItems.Add(i);
		}

		public ObservableCollection<T> TopItems { get; private set; } 
	}

	public class ObservableGroups<T> : ObservableCollection<ExtendedGroup<T>>
	{
		public ExtendedGroup<T> AddGroup(string title)
		{
			return AddGroup(title, new List<T>());
		}

		public ExtendedGroup<T> AddGroup(string title, IEnumerable<T> items)
		{
			var group = new ExtendedGroup<T>(title, items);
			Add(group);
			return group;
		}
	}
}