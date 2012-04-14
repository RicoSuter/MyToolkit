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
	public class ExtendedObservableCollection<T> : IList<T>, ICollection<T>, IReadOnlyList<T>, IEnumerable<T>, IList, ICollection, IEnumerable, 
		INotifyCollectionChanged, INotifyPropertyChanged, IExtendedObservableCollection
	{
		public IList<T> Items { get; private set; }
		private MyObservableCollection<T> internalList = new MyObservableCollection<T>();

		public ExtendedObservableCollection(IList<T> originalCollection)
			: this(originalCollection, null) { }

		public ExtendedObservableCollection(IList<T> originalCollection, Func<T, bool> filter = null,
			Func<T, object> orderBy = null, bool ascending = true, int limit = 0, bool trackItemChanges = false)
		{
			Items = originalCollection;

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

			internalList.CollectionChanged += OnCollectionChanged;
			internalList.MyPropertyChanged += OnPropertyChanged;

			isTracking = true;
			UpdateList();
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
				lock (this)
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
			}
			else
				weakEvent.Target.CollectionChanged -= weakEvent.Event;
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

		private void UpdateList()
		{
			if (!IsTracking)
				return; 
			
			lock (this)
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
					list = ((ObservableCollection<T>)Items).ToList();
				else
					throw new Exception();

				if (Limit > 0 || Offset > 0)
					list = list.Skip(Offset).Take(Limit).ToList();

				foreach (var item in internalList.ToArray()) // remove items
				{
					if (!list.Contains(item))
						internalList.Remove(item);
				}

				for (int i = 0; i < list.Count; i++) // move items
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

		#region interfaces

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var copy = CollectionChanged;
			if (copy != null)
				copy(this, e);
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var copy = PropertyChanged;
			if (copy != null)
				copy(this, e);
		}

		public int Count
		{
			get 
			{ 
				lock (this)
					return internalList.Count; 
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			lock (this)
				return internalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (this)
				return internalList.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			lock (this)
				return internalList.IndexOf(item);
		}

		public T this[int index]
		{
			get 
			{
				lock (this)
					return internalList[index]; 
			}
			set { throw new NotImplementedException(); }
		}

		public bool Contains(T item)
		{
			lock (this)
				return internalList.Contains(item);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Contains(object value)
		{
			lock (this)
				return value is T && internalList.Contains((T)value);
		}

		public int IndexOf(object value)
		{
			if (!(value is T))
				return -1;
			lock (this)
				return internalList.IndexOf((T)value);
		}

		object IList.this[int index]
		{
			get 
			{
				lock (this)
					return internalList[index]; 
			} 
			set { throw new NotImplementedException(); }
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
			get { return null; }
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		int IList.Add(object value)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, object value)
		{
			throw new NotImplementedException();
		}

		public void Remove(object value)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	#region helper classes

	public interface IExtendedObservableCollection : IList, ICollection, INotifyCollectionChanged, INotifyPropertyChanged
	{
		bool IsTracking { get; set; }

		int Limit { get; set; }
		int Offset { get; set; }

		object Order { get; set; }
		bool Ascending { get; set; }
		object Filter { get; set; }
	}

	public class MyObservableCollection<T> : ObservableCollection<T>
	{
		public event PropertyChangedEventHandler MyPropertyChanged
		{
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}
	}

	#endregion
}