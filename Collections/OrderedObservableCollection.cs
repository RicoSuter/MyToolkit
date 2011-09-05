using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyToolkit.Collections
{
	public class OrderedObservableCollection<T, TOrderByKey> : ObservableCollection<T>
	{
		private readonly ObservableCollection<T> originalCollection;
		private readonly Func<T, bool> where; 
		private readonly Func<T, TOrderByKey> orderBy;

		public OrderedObservableCollection(ObservableCollection<T> originalCollection, Func<T, TOrderByKey> orderBy, Func<T, bool> where) 
		{
			this.orderBy = orderBy;
			this.originalCollection = originalCollection;
			this.where = where;

			originalCollection.CollectionChanged += delegate { UpdateList(); };
			UpdateList();
		}

		private void UpdateList()
		{
			var list = originalCollection.Where(where).OrderBy(orderBy).ToList();

			foreach (var item in this.ToArray()) // remove items
			{
				if (!list.Contains(item))
					Remove(item);
			}

			var prev = -1;
			foreach (var item in list) // insert new items
			{
				if (!Contains(item))
				{
					if (prev == -1)
						Insert(0, item);
					else
					{
						var prevItem = list[prev];
						Insert(IndexOf(prevItem) + 1, item);
					}
				}
				prev++;
			}
		}
	}
}