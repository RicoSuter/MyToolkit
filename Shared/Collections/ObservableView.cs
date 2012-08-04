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
	public class ObservableView<T> : IList<T>, IObservableView
	{
		public IList<T> Items { get; private set; }

		NotifyCollectionChangedEventHandler itemsChangedHandler; 

		private ExtendedObservableCollection<T> internalList = new ExtendedObservableCollection<T>();

		public ObservableView()
			: this(new ObservableCollection<T>(), null) { }

		public ObservableView(IList<T> items)
			: this(items, null) { }

		public ObservableView(IList<T> items, Func<T, bool> filter = null,
			Func<T, object> orderBy = null, bool ascending = true, int limit = 0, bool trackItemChanges = false)
		{
			Items = items;

			Limit = limit;
			Order = orderBy;
			Filter = filter;
			Ascending = ascending;

			TrackItemChanges = trackItemChanges;
			TrackCollectionChanges = true; 

			if (TrackItemChanges)
				TrackAllItems();

			internalList.CollectionChanged += OnInternalCollectionChanged;
			internalList.PropertyChanged += OnInternalPropertyChanged;

			isTracking = true;
			UpdateList();
		}

		object IObservableView.Filter
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

		object IObservableView.Order
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

		private int offset;
		public int Offset
		{
			get { return offset; }
			set
			{
				offset = value;
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

		private bool isTracking = false;
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

		private bool trackCollectionChanges;
		public bool TrackCollectionChanges
		{
			get { return trackCollectionChanges; }
			set
			{
				if (value != trackCollectionChanges)
				{
					trackCollectionChanges = value;
					if (trackCollectionChanges)
						TrackCollection();
					else
						UntrackCollection();
					UpdateList();
				}
			}
		}

		private bool trackItemChanges;
		public bool TrackItemChanges
		{
			get { return trackItemChanges; }
			set
			{
				if (value != trackItemChanges)
				{
					trackItemChanges = value;
					if (trackItemChanges)
						TrackAllItems();
					else
						UntrackAllItems();
					UpdateList();
				}
			}
		}

		private void OnOriginalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			lock (SyncRoot)
			{
				UpdateList();
				if (TrackItemChanges)
				{
					if (e.NewItems != null)
					{
						foreach (var i in e.NewItems.OfType<INotifyPropertyChanged>())
							RegisterEvent(i);
					}

					if (e.OldItems != null)
					{
						foreach (var i in e.OldItems.OfType<INotifyPropertyChanged>())
							UnregisterEvent(i);
					}
				}
			}
		}

		private readonly Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler> events =
			new Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler>();

		private void RegisterEvent(INotifyPropertyChanged item)
		{
			if (events.ContainsKey(item))
				return;

			var handler = WeakEvent.Set<ObservableView<T>, PropertyChangedEventHandler, PropertyChangedEventArgs>(
				h => (o, e) => h(o, e),
				h => item.PropertyChanged += h,
				h => item.PropertyChanged -= h,
				this, (s, e) => s.UpdateList());

			events.Add(item, handler);
		}

		private void UnregisterEvent(INotifyPropertyChanged item)
		{
			if (!events.ContainsKey(item))
				return;

			var handler = events[item];
			item.PropertyChanged -= handler;
			events.Remove(item);
		}

		private void TrackCollection()
		{
			if (Items is ObservableCollection<T>)
			{
				var collection = (ObservableCollection<T>)Items;

				itemsChangedHandler = WeakEvent.Set<ObservableView<T>, NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
					h => (o, e) => h(o, e),
					h => collection.CollectionChanged += h,
					h => collection.CollectionChanged -= h,
					this, (s, e) => s.OnOriginalCollectionChanged(s, e));
			}
		}

		private void UntrackCollection()
		{
			if (itemsChangedHandler != null)
			{
				((ObservableCollection<T>)Items).CollectionChanged -= itemsChangedHandler;
				itemsChangedHandler = null;
			}
		}

		private void TrackAllItems()
		{
			foreach (var i in Items.OfType<INotifyPropertyChanged>())
				RegisterEvent(i);
		}

		private void UntrackAllItems()
		{
			foreach (var i in Items.OfType<INotifyPropertyChanged>())
				UnregisterEvent(i);
		}

		private void UpdateList()
		{
			if (!IsTracking)
				return;

			lock (SyncRoot)
			{
				List<T> list;
				if (Filter != null && Order != null && Ascending)
					list = Items.Where(Filter).OrderBy(Order).ToList();
				else if (Filter != null && Order != null && !Ascending)
					list = Items.Where(Filter).OrderByDescending(Order).ToList();
				else if (Filter == null && Order != null && Ascending)
					list = Items.OrderBy(Order).ToList();
				else if (Filter == null && Order != null && !Ascending)
					list = Items.OrderByDescending(Order).ToList();
				else if (Filter != null && Order == null)
					list = Items.Where(Filter).ToList();
				else if (Filter == null && Order == null)
					list = Items.ToList();
				else
					throw new Exception();

				if (Limit > 0 || Offset > 0)
					list = list.Skip(Offset).Take(Limit).ToList();

				foreach (var item in internalList.Where(item => !list.Contains(item)).ToArray())
					internalList.Remove(item);

				for (var i = 0; i < list.Count; i++) // move items
				{
					var item = list[i];
					var oldIndex = internalList.IndexOf(item);
					if (oldIndex != i)
						internalList.Remove(item);
				}

				var prev = -1;
				foreach (var item in list) // insert new items
				{
					if (!internalList.Contains(item))
					{
						if (prev == -1)
							internalList.Insert(0, item);
						else
						{
							var prevItem = list[prev];
							internalList.Insert(internalList.IndexOf(prevItem) + 1, item);
						}
					}
					prev++;
				}

				// TODO: change to this => Move is not implemented in WinRT!
				//for (int i = 0; i < list.Count; i++) // move items
				//{
				//	var item = list[i];
				//	var oldIndex = internalList.IndexOf(item);
				//	if (oldIndex != i)
				//		internalList.Move(oldIndex, i);
				//}
			}
		}

		public void AddRange(IEnumerable<T> collection)
		{
			var old = TrackCollectionChanges; 
			TrackCollectionChanges = false;

			if (Items is ExtendedObservableCollection<T>)
				((ExtendedObservableCollection<T>)Items).AddRange(collection);
			else
			{
				foreach (var i in collection)
					Add(i);
			}

			TrackCollectionChanges = old; 
		}

		public void Close()
		{
			TrackCollectionChanges = false; 
			TrackItemChanges = false;

			internalList = null;
			Items = null;
		}

		#region interfaces

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnInternalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var copy = CollectionChanged;
			if (copy != null)
				copy(this, e);
		}

		private void OnInternalPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var copy = PropertyChanged;
			if (copy != null)
				copy(this, e);
		}

		public int Count
		{
			get 
			{ 
				lock (SyncRoot)
					return internalList.Count; 
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			lock (SyncRoot)
				return internalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (SyncRoot)
				return internalList.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			lock (SyncRoot)
				return internalList.IndexOf(item);
		}

		public T this[int index]
		{
			get 
			{
				lock (SyncRoot)
					return internalList[index]; 
			}
			set { throw new Exception("Method with index not allowed (list may be filtered). Use Items property instead."); }
		}

		object IList.this[int index]
		{
			get
			{
				lock (SyncRoot)
					return internalList[index];
			}
			set { throw new Exception("Method with index not allowed (list may be filtered). Use Items property instead."); }
		}

		public bool Contains(T item)
		{
			lock (SyncRoot)
				return internalList.Contains(item);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Contains(object value)
		{
			lock (SyncRoot)
				return value is T && internalList.Contains((T)value);
		}

		public int IndexOf(object value)
		{
			if (!(value is T))
				return -1;

			lock (SyncRoot)
				return internalList.IndexOf((T)value);
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public bool IsSynchronized
		{
			get { return true; }
		}

		public object SyncRoot
		{
			get { return Items; }
		}

		public void Insert(int index, T item)
		{
			throw new Exception("Method with index not allowed (list may be filtered). Use Items property instead.");
		}

		public void Insert(int index, object value)
		{
			throw new Exception("Method with index not allowed (list may be filtered). Use Items property instead.");
		}

		public void RemoveAt(int index)
		{
			throw new Exception("Method with index not allowed (list may be filtered). Use Items property instead.");
		}

		int IList.Add(object value)
		{
			throw new Exception("Method with index not allowed (list may be filtered). Use Items property instead.");
		}

		public void Add(T item)
		{
			lock (SyncRoot)
				Items.Add(item);
		}

		public void Clear()
		{
			lock (SyncRoot)
				Items.Clear();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			lock (SyncRoot)
				Items.CopyTo(array, arrayIndex);
		}

		public void CopyTo(Array array, int index)
		{
			CopyTo((T[])array, index);
		}

		public bool Remove(T item)
		{
			lock (SyncRoot)
				return Items.Remove(item);
		}

		public void Remove(object value)
		{
			Remove((T)value);
		}

		#endregion
	}

	#region helper classes

	public interface IObservableView : IList, INotifyCollectionChanged, INotifyPropertyChanged
	{
		bool IsTracking { get; set; }

		int Limit { get; set; }
		int Offset { get; set; }

		object Order { get; set; }
		bool Ascending { get; set; }
		object Filter { get; set; }
	}

	#endregion
}