using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MyToolkit.Utilities;

namespace MyToolkit.Collections
{
#if METRO
	public class ExtendedObservableCollection<T> : ExtendedObservableCollection<T, object>
	{
		public ExtendedObservableCollection(ObservableVector<T> originalCollection, Func<T, bool> filter = null)
			: base (originalCollection, filter) { }
	}

	public class ExtendedObservableCollection<T, TOrderByKey> : ObservableVector<T>
	{
		private readonly ObservableVector<T> originalCollection;

		public ExtendedObservableCollection(ObservableVector<T> originalCollection, Func<T, bool> filter = null,
			Func<T, TOrderByKey> orderBy = null, bool ascending = true, int limit = 0, bool trackItemChanges = false)
		{
#else
	public class ExtendedObservableCollection<T> : ExtendedObservableCollection<T, object>
	{
		public ExtendedObservableCollection(ObservableCollection<T> originalCollection, Func<T, bool> filter = null)
			: base (originalCollection, filter) { }
	}
	
	public class ExtendedObservableCollection<T, TOrderByKey> : ObservableCollection<T>
	{
		private readonly ObservableCollection<T> originalCollection;

		public ExtendedObservableCollection(ObservableCollection<T> originalCollection, Func<T, bool> filter = null,
			Func<T, TOrderByKey> orderBy = null, bool ascending = true, int limit = 0, bool trackItemChanges = false)
		{
#endif

			IsTracking = false; 
			this.originalCollection = originalCollection;

			Limit = limit;
			Order = orderBy;
			Filter = filter;
			Ascending = ascending;
			TrackItemChanges = trackItemChanges; 

			var weakEvent = new WeakEvent<ObservableCollection<T>, ExtendedObservableCollection<T, TOrderByKey>, 
				NotifyCollectionChangedEventHandler>(originalCollection, this);

			weakEvent.Event = (s, e) => OnCollectionChanged(weakEvent, e);
			originalCollection.CollectionChanged += weakEvent.Event;

			if (TrackItemChanges)
			{
				foreach (var i in originalCollection)
				{
					if (i is INotifyPropertyChanged)
						RegisterEvent((INotifyPropertyChanged)i);
				}
			}

			IsTracking = true; // calls UpdateList()
		}

		public bool TrackItemChanges { get; private set; }

		private Func<T, bool> filter; 
		public Func<T, bool> Filter
		{
			get { return filter; }
			set
			{
				filter = value; 
				UpdateList();
			}
		}

		private Func<T, TOrderByKey> order;
		public Func<T, TOrderByKey> Order
		{
			get { return order; }
			set
			{
				order = value;
				UpdateList();
			}
		}
		
		private int limit;
		public int Limit
		{
			get { return limit; }
			set
			{
				limit = value;
				UpdateList();
			}
		}

		private bool ascending = true;
		public bool Ascending
		{
			get { return ascending; }
			set
			{
				ascending = value;
				UpdateList();
			}
		}

		private bool isTracking;
		public bool IsTracking
		{
			get { return isTracking; }
			set 
			{
				isTracking = value;
				if (value)
					UpdateList();
			}
		}

		private readonly Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler> events =
			new Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler>();
		private void RegisterEvent(INotifyPropertyChanged item)
		{
			if (events.ContainsKey(item))
				return;

			var weakEvent = new WeakEvent<INotifyPropertyChanged, ExtendedObservableCollection<T, TOrderByKey>,
				PropertyChangedEventHandler>(originalCollection, this);
			weakEvent.Event = (s, e) => OnItemChanged(weakEvent);
			events.Add(item, weakEvent.Event);
			item.PropertyChanged += weakEvent.Event;
		}

		private void OnItemChanged(WeakEvent<INotifyPropertyChanged, ExtendedObservableCollection<T, TOrderByKey>,
				PropertyChangedEventHandler> weakEvent)
		{
			if (weakEvent.IsAlive)
				weakEvent.Reference.UpdateList();
			else
				weakEvent.Target.PropertyChanged -= weakEvent.Event;
		}

#if METRO
		private void OnCollectionChanged(WeakEvent<ObservableCollection<T>, ExtendedObservableCollection<T, TOrderByKey>,
			NotifyCollectionChangedEventHandler> weakEvent, NotifyCollectionChangedEventArgs e)
		{
#else
		private void OnCollectionChanged(WeakEvent<ObservableCollection<T>, ExtendedObservableCollection<T, TOrderByKey>,
			NotifyCollectionChangedEventHandler> weakEvent, NotifyCollectionChangedEventArgs e)
		{
#endif	
			if (weakEvent.IsAlive)
			{
				weakEvent.Reference.UpdateList();
				if (TrackItemChanges)
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

		private void UpdateList()
		{
			if (!IsTracking)
				return;

			List<T> list;
			if (Filter != null && Order != null && Ascending)
				list = originalCollection.Where(Filter).OrderBy(Order).ToList();
			else if (Filter != null && Order != null && !Ascending)
				list = originalCollection.Where(Filter).OrderByDescending(Order).ToList();
			else if (Filter == null && Order != null && Ascending)
				list = originalCollection.OrderBy(Order).ToList();
			else if (Filter == null && Order != null && !Ascending)
				list = originalCollection.OrderByDescending(Order).ToList();
			else if (Filter != null && Order == null)
				list = originalCollection.Where(Filter).ToList();
			else if (Filter == null && Order == null)
				list = ((ObservableCollection<T>)originalCollection).ToList();
			else
				throw new Exception();

			if (Limit > 0)
				list = list.Take(Limit).ToList();

			foreach (var item in ((ObservableCollection<T>)this).ToArray()) // remove items
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