using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MyToolkit.Utilities;

#if METRO
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace MyToolkit.Collections
{
	public interface IExtendedObservableCollection : IList, INotifyCollectionChanged, INotifyPropertyChanged
	{
		bool IsTracking { get; set; }
		int Limit { get; set; }

		object Order { get; set; }
		bool Ascending { get; set; }
		object Filter { get; set; }
	}

	public class ExtendedObservableCollection<T> : ObservableCollection<T>, IExtendedObservableCollection
	{
		private readonly IList<T> originalCollection;

		public ExtendedObservableCollection(IList<T> originalCollection)
			: this(originalCollection, null) { }

		public ExtendedObservableCollection(IList<T> originalCollection, Func<T, bool> filter = null,
			Func<T, object> orderBy = null, bool ascending = true, int limit = 0, bool trackItemChanges = false)
		{
			IsTracking = false; 
			this.originalCollection = originalCollection;

			Limit = limit;
			Order = orderBy;
			Filter = filter;
			Ascending = ascending;
			TrackItemChanges = trackItemChanges;

			if (originalCollection is ObservableCollection<T>)
			{
				var collection = (ObservableCollection<T>)originalCollection;
				var weakEvent = new WeakEvent<ObservableCollection<T>, ExtendedObservableCollection<T>,	NotifyCollectionChangedEventHandler>(collection, this);

				weakEvent.Event = (s, e) => OnCollectionChanged(weakEvent, e);
				collection.CollectionChanged += weakEvent.Event;
			}

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

		object IExtendedObservableCollection.Filter
		{
			get { return Filter; }
			set { Filter = (Func<T, bool>)value; }
		}

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

		object IExtendedObservableCollection.Order
		{
			get { return Order; }
			set { Order = (Func<T, object>)value; }
		}

		private Func<T, object> order;
		public Func<T, object> Order
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

			var weakEvent = new WeakEvent<INotifyPropertyChanged, ExtendedObservableCollection<T>, PropertyChangedEventHandler>(item, this);
			weakEvent.Event = (s, e) => OnItemChanged(weakEvent);

			events.Add(item, weakEvent.Event);
			item.PropertyChanged += weakEvent.Event;
		}

		private void OnItemChanged(WeakEvent<INotifyPropertyChanged, ExtendedObservableCollection<T>, PropertyChangedEventHandler> weakEvent)
		{
			if (weakEvent.IsAlive)
				weakEvent.Reference.UpdateList();
			else
				weakEvent.Target.PropertyChanged -= weakEvent.Event;
		}

		private void OnCollectionChanged(WeakEvent<ObservableCollection<T>, ExtendedObservableCollection<T>,
			NotifyCollectionChangedEventHandler> weakEvent, NotifyCollectionChangedEventArgs e)
		{
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
				if (base.IndexOf(item) != i)
					base.Remove(item);
			}

			var prev = -1;
			foreach (var item in list) // insert new items
			{
				if (!base.Contains(item))
				{
					if (prev == -1)
					{
#if METRO
						try { base.Insert(0, item); }
						catch { } // TODO: WinRT hack => solve problem
#else
						base.Insert(0, item);
#endif
					}
					else
					{
#if METRO
						try
						{
							var prevItem = list[prev];
							base.Insert(base.IndexOf(prevItem) + 1, item);
						}
						catch { } // TODO: WinRT hack => solve problem
#else
						var prevItem = list[prev];
						base.Insert(base.IndexOf(prevItem) + 1, item);
#endif
					}
				}
				prev++;
			}
		}
	}
}