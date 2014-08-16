//-----------------------------------------------------------------------
// <copyright file="ObservableCollectionView.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MyToolkit.Utilities;

namespace MyToolkit.Collections
{
	/// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with automatic sorting and filtering. </summary>
	/// <typeparam name="T">The type of an item. </typeparam>
    public class ObservableCollectionView<T> : ObservableView<T>
    {
        public ObservableCollectionView() : this(new ObservableCollection<T>(), null) { }
		public ObservableCollectionView(IList<T> items) : this(items, null) { }
        public ObservableCollectionView(IList<T> items, Func<T, bool> filter = null,
			Func<T, object> orderBy = null, bool ascending = true, int limit = 0, bool trackItemChanges = false)
            : base(items, filter, orderBy, ascending, limit, trackItemChanges)
		{
		}
    }

	/// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with automatic sorting and filtering. </summary>
	/// <typeparam name="T">The type of an item. </typeparam>
    [Obsolete("Use ObservableCollectionView instead. 5/19/2014")]
	public class ObservableView<T> : IList<T>, IObservableCollectionView, IDisposable
	{
		private NotifyCollectionChangedEventHandler _itemsChangedHandler; 
		private ExtendedObservableCollection<T> _internalList = new ExtendedObservableCollection<T>();
        private readonly Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler> _events =
            new Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler>();

        private Func<T, bool> _filter;
        private Func<T, object> _order;

        private int _offset;
        private int _limit;
        private bool _ascending = true;

        private bool _isTracking = false;
        private bool _trackItemChanges;
        private bool _trackCollectionChanges = false;

		public ObservableView() : this(new ObservableCollection<T>(), null) { }
		public ObservableView(IList<T> items) : this(items, null) { }
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

			_internalList.CollectionChanged += OnInternalCollectionChanged;
			_internalList.PropertyChanged += OnInternalPropertyChanged;

			_isTracking = true;
			UpdateList();
		}

        /// <summary>
        /// Gets the original items source. 
        /// </summary>
        public IList<T> Items { get; private set; }

        /// <summary>
        /// Gets or sets the filter (a Func{TItem, bool} object). 
        /// </summary>
        object IObservableView.Filter
		{
			get { return Filter; }
			set { Filter = (Func<T, bool>)value; }
		}

		/// <summary>
		/// Gets or sets the filter. 
		/// </summary>
		public Func<T, bool> Filter
		{
			get { return _filter; }
			set
			{
				_filter = value; 
				UpdateList();
			}
		}

        /// <summary>
        /// Gets or sets the order. 
        /// </summary>
        object IObservableView.Order
		{
			get { return Order; }
			set { Order = (Func<T, object>)value; }
		}

		/// <summary>
		/// Gets or sets the sorting/order function
		/// </summary>
		public Func<T, object> Order
		{
			get { return _order; }
			set
			{
				_order = value;
				UpdateList();
			}
		}

        /// <summary>
        /// Gets or sets the maximum number of items in the view. 
        /// </summary>
        public int Limit
		{
			get { return _limit; }
			set
			{
				_limit = value;
				UpdateList();
			}
		}

		/// <summary>
		/// Gets or sets the offset from where the results a selected. 
		/// </summary>
		public int Offset
		{
			get { return _offset; }
			set
			{
				_offset = value;
				UpdateList();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the sorting should be ascending; otherwise descending. 
		/// </summary>
		public bool Ascending
		{
			get { return _ascending; }
			set
			{
				_ascending = value;
				UpdateList();
			}
		}

		/// <summary>
		/// Gets or sets a flag whether the view should automatically be updated when needed. 
		/// Disable this flag when doing multiple of operations on the underlying collection. 
		/// Enabling this flag automatically updates the view if needed. 
		/// </summary>
		public bool IsTracking
		{
			get { return _isTracking; }
			set 
			{
				_isTracking = value;
				if (value)
					UpdateList();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the view should listen for collection 
		/// changed events on the underlying collection (default: true). 
		/// </summary>
		public bool TrackCollectionChanges
		{
			get { return _trackCollectionChanges; }
			set
			{
				if (value != _trackCollectionChanges)
				{
					_trackCollectionChanges = value;
					if (_trackCollectionChanges)
						TrackCollection();
					else
						UntrackCollection();
					UpdateList();
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the items in the collection should be tracked for property changes. 
		/// The items must implement INotifyPropertyChanged to support item tracking. 
        /// Enable this property if your items are mutable and the list has to be restored if an item property changes. 
        /// </summary>
		public bool TrackItemChanges
		{
			get { return _trackItemChanges; }
			set
			{
				if (value != _trackItemChanges)
				{
					_trackItemChanges = value;
					if (_trackItemChanges)
						TrackAllItems();
					else
						UntrackAllItems();
					UpdateList();
				}
			}
		}

        /// <summary>
        /// Adds a multiple elements to the underlying collection. 
        /// </summary>
        /// <param name="collection"></param>
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

        /// <summary>
        /// Releases all used resources and deregisters all events on the items and the underlying collection. 
        /// </summary>
        public void Dispose()
        {
            TrackCollectionChanges = false;
            TrackItemChanges = false;

            _internalList = null;
            Items = null;
        }

        /// <summary>
        /// Releases all used resources and deregisters all events on the items and the underlying collection. 
        /// </summary>
        [Obsolete("Use Dispose instead. 5/17/2014")]
        public void Close()
        {
            Dispose();
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

					if (e.Action == NotifyCollectionChangedAction.Reset)
					{
						foreach (var item in _events.Select(p => p.Key).ToArray())
							UnregisterEvent(item);
					}
				}
			}
		}

		private void RegisterEvent(INotifyPropertyChanged item)
		{
			if (_events.ContainsKey(item))
				return;

            var handler = WeakEvent.Register<ObservableView<T>, PropertyChangedEventHandler, PropertyChangedEventArgs>(
                this, 
                h => item.PropertyChanged += h, 
                h => item.PropertyChanged -= h, 
                h => (o, e) => h(o, e),
                (subscriber, s, e) => subscriber.UpdateList());

			_events.Add(item, handler);
		}

		private void UnregisterEvent(INotifyPropertyChanged item)
		{
			if (!_events.ContainsKey(item))
				return;

			var handler = _events[item];
			item.PropertyChanged -= handler;
			_events.Remove(item);
		}

		private void TrackCollection()
		{
			if (Items is ObservableCollection<T>)
			{
				var collection = (ObservableCollection<T>)Items;
                _itemsChangedHandler = WeakEvent.Register<ObservableView<T>, NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    this,
                    h => collection.CollectionChanged += h, 
                    h => collection.CollectionChanged -= h, 
                    h => (o, e) => h(o, e),
                    (subscriber, s, e) => subscriber.OnOriginalCollectionChanged(s, e));
			}
		}

		private void UntrackCollection()
		{
			if (_itemsChangedHandler != null)
			{
				((ObservableCollection<T>)Items).CollectionChanged -= _itemsChangedHandler;
				_itemsChangedHandler = null;
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

        /// <summary>
        /// Updates the view. 
        /// </summary>
		public void Update()
		{
			UpdateList();
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

                // Remove items
				foreach (var item in _internalList.Where(item => !list.Contains(item)).ToArray())
					_internalList.Remove(item);

#if LEGACY
                // Move items (by deleting
                for (var i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var oldIndex = _internalList.IndexOf(item);
                    if (oldIndex != i)
                        _internalList.Remove(item);
                }
#endif

                // Insert new items
				var prev = -1;
				foreach (var item in list) 
				{
					if (!_internalList.Contains(item))
					{
						if (prev == -1)
							_internalList.Insert(0, item);
						else
						{
							var prevItem = list[prev];
							_internalList.Insert(_internalList.IndexOf(prevItem) + 1, item);
						}
					}
					prev++;
				}

#if !LEGACY
                // Move items
				for (int i = 0; i < list.Count; i++) 
				{
					var item = list[i];
					var oldIndex = _internalList.IndexOf(item);
					if (oldIndex != i)
						_internalList.Move(oldIndex, i);
				}
#endif
			}
		}

		#region Interfaces

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
					return _internalList.Count; 
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			lock (SyncRoot)
				return _internalList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			lock (SyncRoot)
				return _internalList.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			lock (SyncRoot)
				return _internalList.IndexOf(item);
		}

		public T this[int index]
		{
			get 
			{
				lock (SyncRoot)
					return _internalList[index]; 
			}
			set { throw new Exception("Method with index not allowed (list may be filtered). Use Items property instead."); }
		}

		object IList.this[int index]
		{
			get
			{
				lock (SyncRoot)
					return _internalList[index];
			}
			set { throw new Exception("Method with index not allowed (list may be filtered). Use Items property instead."); }
		}

		public bool Contains(T item)
		{
			lock (SyncRoot)
				return _internalList.Contains(item);
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Contains(object value)
		{
			lock (SyncRoot)
				return value is T && _internalList.Contains((T)value);
		}

		public int IndexOf(object value)
		{
			if (!(value is T))
				return -1;

			lock (SyncRoot)
				return _internalList.IndexOf((T)value);
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

    public interface IObservableCollectionView : IObservableView
    {
    }

    [Obsolete("Use IObservableCollectionView instead. 5/19/2014")]
    public interface IObservableView 
		: IList, INotifyCollectionChanged, INotifyPropertyChanged
	{
        /// <summary>
        /// Gets or sets a value indicating whether the view should automatically be updated when needed. 
        /// Disable this flag when doing multiple of operations on the underlying collection. 
        /// Enabling this flag automatically updates the view if needed. 
        /// </summary>
        bool IsTracking { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items in the view. 
        /// </summary>
		int Limit { get; set; }

        /// <summary>
        /// Gets or sets the offset from where the results a selected. 
        /// </summary>
        int Offset { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to sort ascending or descending. 
        /// </summary>
		bool Ascending { get; set; }

        /// <summary>
        /// Gets or sets the filter (a Func{TItem, bool} object). 
        /// </summary>
		object Filter { get; set; }
    
        /// <summary>
        /// Gets or sets the order (a Func{TItem, object} object). 
        /// </summary>
        object Order { get; set; }
    }
}