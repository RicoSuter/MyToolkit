using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MyToolkit.Utilities;

namespace MyToolkit.Collections
{
	public class OrderedObservableCollection<T, TOrderByKey> : ObservableCollection<T>
	{
		private readonly ObservableCollection<T> originalCollection;
		private Func<T, bool> where; 
		private Func<T, TOrderByKey> orderBy;
		private int limit;
		private bool ascending = true;
		private bool trackItemChanges = false;

		public OrderedObservableCollection(ObservableCollection<T> originalCollection, Func<T, TOrderByKey> orderBy, 
			Func<T, bool> where = null, bool ascending = true, int limit = 0, bool trackItemChanges = false)
		{
			this.limit = limit; 
			this.orderBy = orderBy;
			this.originalCollection = originalCollection;
			this.where = where;
			this.ascending = ascending;
			this.trackItemChanges = trackItemChanges; 

			var weakEvent = new WeakEvent<ObservableCollection<T>, OrderedObservableCollection<T, TOrderByKey>, 
				NotifyCollectionChangedEventHandler>(originalCollection, this);
			weakEvent.Event = (s, e) => OnCollectionChanged(weakEvent, e);
			originalCollection.CollectionChanged += weakEvent.Event;

			if (trackItemChanges)
			{
				foreach (var i in originalCollection)
				{
					if (i is INotifyPropertyChanged)
						RegisterEvent((INotifyPropertyChanged)i);
				}
			}

			UpdateList();
		}

		private readonly Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler> events = 
			new Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler>();
		private void RegisterEvent(INotifyPropertyChanged item)
		{
			if (events.ContainsKey(item))
				return;

			var weakEvent = new WeakEvent<INotifyPropertyChanged, OrderedObservableCollection<T, TOrderByKey>,
				PropertyChangedEventHandler>(originalCollection, this);
			weakEvent.Event = (s, e) => OnItemChanged(weakEvent);
			events.Add(item, weakEvent.Event);
			item.PropertyChanged += weakEvent.Event;
		}

		private void OnItemChanged(WeakEvent<INotifyPropertyChanged, OrderedObservableCollection<T, TOrderByKey>,
				PropertyChangedEventHandler> weakEvent)
		{
			if (weakEvent.IsAlive)
				weakEvent.Reference.UpdateList();
			else
				weakEvent.Target.PropertyChanged -= weakEvent.Event;
		}

		private void OnCollectionChanged(WeakEvent<ObservableCollection<T>, OrderedObservableCollection<T, TOrderByKey>, 
			NotifyCollectionChangedEventHandler> weakEvent, NotifyCollectionChangedEventArgs e)
		{
			if (weakEvent.IsAlive)
			{
				weakEvent.Reference.UpdateList();
				if (trackItemChanges)
				{
					if (e.NewItems != null)
					{
						foreach (var i in e.NewItems)
						{
							if (i is INotifyPropertyChanged)
								RegisterEvent((INotifyPropertyChanged)i);
						}
					}
					if (e.OldItems != null)
					{
						foreach (var i in e.OldItems)
						{
							if (i is INotifyPropertyChanged && events.ContainsKey((INotifyPropertyChanged)i))
							{
								var ev = events[(INotifyPropertyChanged)i];
								((INotifyPropertyChanged)i).PropertyChanged -= ev;
							}
						}
					}
				}
			}
			else
				weakEvent.Target.CollectionChanged -= weakEvent.Event;
		}

		public void ResetFilter(Func<T, bool> where)
		{
			this.where = where;
			UpdateList();
		}

		public void ResetOrder(Func<T, TOrderByKey> orderBy)
		{
			this.orderBy = orderBy;
			UpdateList();
		}

		public void Reset(Func<T, TOrderByKey> orderBy, Func<T, bool> where)
		{
			this.orderBy = orderBy;
			this.where = where;
			UpdateList();
		}

		private void UpdateList()
		{
			var list = where != null ? 
				originalCollection.Where(where).OrderBy(orderBy).ToList() :
				(ascending ? 
					originalCollection.OrderBy(orderBy).ToList() :
					originalCollection.OrderByDescending(orderBy).ToList());

			if (limit > 0)
				list = list.Take(limit).ToList(); // TODO oben einbauen 

			foreach (var item in this.ToArray()) // remove items
			{
				if (!list.Contains(item))
					Remove(item);
			}

			for (int i = 0; i < list.Count; i++) // moved => TODO better
			{
				var item = list[i];
				if (IndexOf(item) != i)
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