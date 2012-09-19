using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyToolkit.Collections
{
	// THIS IS BETA!
	public class NotifyPropertyChangedListListener<T> where T : INotifyPropertyChanged
	{
		private ObservableCollection<T> list;
		private readonly PropertyChangedEventHandler handler;
		private readonly List<T> registeredItems = new List<T>();

		public event EventHandler<ExtendedNotifyCollectionChangedEventArgs<T>> CollectionChanged;

		public NotifyPropertyChangedListListener(PropertyChangedEventHandler handler, ObservableCollection<T> list = null)
		{
			this.handler = handler;
			Reset(list);
		}

		public void Reset(ObservableCollection<T> newList)
		{
			if (newList == list)
				return;

			if (list != null)
			{
				list.CollectionChanged -= ListOnCollectionChanged;
				foreach (var i in registeredItems)
					i.PropertyChanged -= handler;
			}

			list = newList;
			if (list != null)
			{
				list.CollectionChanged += ListOnCollectionChanged;
				foreach (var i in list)
				{
					registeredItems.Add(i);
					i.PropertyChanged += handler;
				}
			}
		}

		private void ListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			var addedItems = new List<T>();
			foreach (var i in list.Where(x => !registeredItems.Contains(x))) // new itemss
			{
				addedItems.Add(i);
				registeredItems.Add(i);
				i.PropertyChanged += handler;
			}

			var removedItems = new List<T>();
			foreach (var i in registeredItems.Where(x => !list.Contains(x)).ToArray()) // deleted items
			{
				removedItems.Add(i);
				registeredItems.Remove(i);
				i.PropertyChanged -= handler;
			}

			var copy = CollectionChanged;
			if (copy != null)
				copy(sender, new ExtendedNotifyCollectionChangedEventArgs<T> { AddedItems = addedItems, RemovedItems = removedItems });
		}
	}
}
