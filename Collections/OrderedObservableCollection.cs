using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MyToolkit.Utilities;

namespace MyToolkit.Collections
{
	public class OrderedObservableCollection<T, TOrderByKey> : ObservableCollection<T>
	{
		private readonly ObservableCollection<T> originalCollection;
		private readonly Func<T, bool> where; 
		private readonly Func<T, TOrderByKey> orderBy;

		public OrderedObservableCollection(ObservableCollection<T> originalCollection, Func<T, TOrderByKey> orderBy, Func<T, bool> where = null) 
		{
			this.orderBy = orderBy;
			this.originalCollection = originalCollection;
			this.where = where;

			var weakEvent = new WeakEvent<ObservableCollection<T>, OrderedObservableCollection<T, TOrderByKey>, NotifyCollectionChangedEventHandler>(originalCollection, this);
			weakEvent.Event = (s, e) => OnCollectionChanged(weakEvent);
			originalCollection.CollectionChanged += weakEvent.Event;

			UpdateList();
		}

		private static void OnCollectionChanged(
			WeakEvent<ObservableCollection<T>, OrderedObservableCollection<T, TOrderByKey>, NotifyCollectionChangedEventHandler> weakEvent)
		{
			if (weakEvent.IsAlive)
				weakEvent.Reference.UpdateList();
			else
				weakEvent.Target.CollectionChanged -= weakEvent.Event;
		}

		private void UpdateList()
		{
			var list = where != null ? 
				originalCollection.Where(where).OrderBy(orderBy).ToList() :
				originalCollection.OrderBy(orderBy).ToList();

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