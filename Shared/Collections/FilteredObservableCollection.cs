using System;
using System.Collections.Generic;
using System.Linq; 
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MyToolkit.Utilities;

namespace MyToolkit.Collections
{
	public class FilteredObservableCollection<T> : ObservableCollection<T>
	{
		private readonly ObservableCollection<T> originalCollection;
		private Func<T, bool> where;

		public FilteredObservableCollection(ObservableCollection<T> originalCollection, Func<T, bool> where)
		{
			this.originalCollection = originalCollection;
			this.where = where;

			var weakEvent = new WeakEvent<ObservableCollection<T>, FilteredObservableCollection<T>, NotifyCollectionChangedEventHandler>(originalCollection, this);
			weakEvent.Event = (s, e) => OnCollectionChanged(weakEvent);
			originalCollection.CollectionChanged += weakEvent.Event;

			UpdateList();
		}

		private static void OnCollectionChanged(
			WeakEvent<ObservableCollection<T>, FilteredObservableCollection<T>, NotifyCollectionChangedEventHandler> weakEvent)
		{
			if (weakEvent.IsAlive)
				weakEvent.Reference.UpdateList();
			else
				weakEvent.Target.CollectionChanged -= weakEvent.Event;
		}

		public void Reset(Func<T, bool> where)
		{
			this.where = where;
			UpdateList();
		}

		private void UpdateList()
		{
			var list = originalCollection.Where(where).ToList();
			
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
