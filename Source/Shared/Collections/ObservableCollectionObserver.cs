using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyToolkit.Collections
{
	// THIS IS BETA!
	public class ObservableCollectionObserver<T> where T : INotifyPropertyChanged
	{
		private ObservableCollection<T> list;
		private readonly List<T> registeredItems = new List<T>();

		public event EventHandler<ExtendedNotifyCollectionChangedEventArgs<T>> CollectionChanged;
		public event PropertyChangedEventHandler ItemChanged;

		public ObservableCollectionObserver(ObservableCollection<T> list = null)
		{
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
					i.PropertyChanged -= OnPropertyChanged;
				registeredItems.Clear();
			}

			list = newList;
			if (list != null)
			{
				list.CollectionChanged += ListOnCollectionChanged;
				foreach (var i in list)
				{
					registeredItems.Add(i);
					i.PropertyChanged += OnPropertyChanged;
				}
			}
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			var copy = ItemChanged;
			if (copy != null)
				copy(sender, args);
		}

		private void ListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			var addedItems = new List<T>();
			foreach (var i in list.Where(x => !registeredItems.Contains(x))) // new items
			{
				addedItems.Add(i);
				registeredItems.Add(i);
				i.PropertyChanged += OnPropertyChanged;
			}

			var removedItems = new List<T>();
			foreach (var i in registeredItems.Where(x => !list.Contains(x)).ToArray()) // deleted items
			{
				removedItems.Add(i);
				registeredItems.Remove(i);
				i.PropertyChanged -= OnPropertyChanged;
			}

			var copy = CollectionChanged;
			if (copy != null)
				copy(sender, new ExtendedNotifyCollectionChangedEventArgs<T> { AddedItems = addedItems, RemovedItems = removedItems });
		}
	}
}
