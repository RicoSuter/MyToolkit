//-----------------------------------------------------------------------
// <copyright file="ObservableCollectionViewBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MyToolkit.Events;
using MyToolkit.Utilities;

namespace MyToolkit.Collections
{
    /// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with automatic sorting and filtering. </summary>
    /// <typeparam name="TItem">The type of an item. </typeparam>
    public abstract class ObservableCollectionViewBase<TItem> : IList<TItem>, IDisposable, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private NotifyCollectionChangedEventHandler _itemsChangedHandler;
        private MtObservableCollection<TItem> _internalCollection = new MtObservableCollection<TItem>();
        private readonly Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler> _events =
            new Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler>();

        private readonly object _syncRoot = new object();
        private bool _isTracking;
        private bool _trackItemChanges;
        private bool _trackCollectionChanges;

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionViewBase{TItem}"/> class. </summary>
        protected ObservableCollectionViewBase()
            : this(new ObservableCollection<TItem>(), false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionViewBase{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        protected ObservableCollectionViewBase(IList<TItem> items)
            : this(items, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionViewBase{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="trackItemChanges">The value indicating whether to track items which implement <see cref="INotifyPropertyChanged"/></param>
        protected ObservableCollectionViewBase(IList<TItem> items, bool trackItemChanges)
        {
            Items = items;

            TrackItemChanges = trackItemChanges;
            TrackCollectionChanges = true;

            if (TrackItemChanges)
                TrackAllItems();

            _internalCollection.CollectionChanged += OnInternalCollectionChanged;
            _internalCollection.PropertyChanged += OnInternalPropertyChanged;

            _isTracking = true;
            Refresh();
        }

        /// <summary>Gets the original items source. </summary>
        public IList<TItem> Items { get; private set; }

        /// <summary>Gets or sets a flag whether the view should automatically be updated when needed. 
        /// Disable this flag when doing multiple of operations on the underlying collection. 
        /// Enabling this flag automatically updates the view if needed. </summary>
        public bool IsTracking
        {
            get { return _isTracking; }
            set
            {
                _isTracking = value;
                if (value)
                    Refresh();
            }
        }

        /// <summary>Gets or sets a value indicating whether the view should listen for collection 
        /// changed events on the underlying collection (default: true). </summary>
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
                    Refresh();
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether the items in the collection should be tracked for property changes. 
        /// The items must implement <see cref="INotifyPropertyChanged"/> to support item tracking. 
        /// Enable this property if your items are mutable and the list has to be restored if an item property changes. </summary>
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
                    Refresh();
                }
            }
        }

        /// <summary>Adds a multiple elements to the underlying collection. </summary>
        /// <param name="items">The items to add. </param>
        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        public void AddRange(IEnumerable<TItem> items)
        {
            var old = TrackCollectionChanges;
            TrackCollectionChanges = false;

            var collection = Items as MtObservableCollection<TItem>;
            if (collection != null)
                collection.AddRange(items);
            else
            {
                foreach (var i in items)
                    Add(i);
            }

            TrackCollectionChanges = old;
        }

        /// <summary>Releases all used resources and deregisters all events on the items and the underlying collection. </summary>
        public void Dispose()
        {
            TrackCollectionChanges = false;
            TrackItemChanges = false;

            _internalCollection = null;
            Items = null;
        }

        /// <summary>Releases all used resources and deregisters all events on the items and the underlying collection. </summary>
        [Obsolete("Use Dispose instead. 5/17/2014")]
        public void Close()
        {
            Dispose();
        }

        /// <summary>Refreshes the view. </summary>
        [Obsolete("Use Refresh method instead. 10/19/2014")]
        public void Update()
        {
            Refresh();
        }

        /// <summary>Refreshes the view. </summary>
        public void Refresh()
        {
            if (!IsTracking)
                return;

            lock (SyncRoot)
            {
                var list = GetItems();
                if (!_internalCollection.IsCopyOf(list))
                    _internalCollection.Initialize(list);
            }
        }

        /// <summary>Gets the list of items with the current order and filter.</summary>
        /// <returns>The items. </returns>
        protected abstract IList<TItem> GetItems();

        private void OnOriginalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (SyncRoot)
            {
                Refresh();

                if (TrackItemChanges)
                {
                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        UntrackAllItems();
                        TrackAllItems();
                    }
                    else
                    {
                        if (e.NewItems != null)
                        {
                            foreach (var item in e.NewItems.OfType<INotifyPropertyChanged>())
                                RegisterEvent(item);
                        }

                        if (e.OldItems != null)
                        {
                            foreach (var item in e.OldItems.OfType<INotifyPropertyChanged>())
                                DeregisterEvent(item);
                        }
                    }
                }
            }
        }

        private void RegisterEvent(INotifyPropertyChanged item)
        {
            if (_events.ContainsKey(item))
                return;

            var handler = WeakEvent.RegisterEvent<ObservableCollectionViewBase<TItem>, PropertyChangedEventHandler, PropertyChangedEventArgs>(
                this,
                h => item.PropertyChanged += h,
                h => item.PropertyChanged -= h,
                h => (o, e) => h(o, e),
                (subscriber, s, e) => subscriber.Refresh());

            _events.Add(item, handler);
        }

        private void DeregisterEvent(INotifyPropertyChanged item)
        {
            if (!_events.ContainsKey(item))
                return;

            var handler = _events[item];
            item.PropertyChanged -= handler;
            _events.Remove(item);
        }

        private void TrackCollection()
        {
            var items = Items as ObservableCollection<TItem>;
            if (items != null)
            {
                var collection = items;
                _itemsChangedHandler = WeakEvent.RegisterEvent<ObservableCollectionViewBase<TItem>, NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
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
                ((ObservableCollection<TItem>)Items).CollectionChanged -= _itemsChangedHandler;
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
            foreach (var item in _events.Keys.ToArray())
                DeregisterEvent(item);
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
                    return _internalCollection.Count;
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            lock (SyncRoot)
                return _internalCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (SyncRoot)
                return _internalCollection.GetEnumerator();
        }

        public int IndexOf(TItem item)
        {
            lock (SyncRoot)
                return _internalCollection.IndexOf(item);
        }

        public TItem this[int index]
        {
            get
            {
                lock (SyncRoot)
                    return _internalCollection[index];
            }
            set { throw new NotSupportedException("Use ObservableCollectionViewBase.Items[] instead."); }
        }

        object IList.this[int index]
        {
            get
            {
                lock (SyncRoot)
                    return _internalCollection[index];
            }
            set { throw new NotSupportedException("Use ObservableCollectionViewBase.Items[] instead."); }
        }

        public bool Contains(TItem item)
        {
            lock (SyncRoot)
                return _internalCollection.Contains(item);
        }

        public bool IsReadOnly { get { return true; } }

        public bool Contains(object value)
        {
            lock (SyncRoot)
                return value is TItem && _internalCollection.Contains((TItem)value);
        }

        public int IndexOf(object value)
        {
            if (!(value is TItem))
                return -1;

            lock (SyncRoot)
                return _internalCollection.IndexOf((TItem)value);
        }

        public bool IsFixedSize { get { return false; } }

        public bool IsSynchronized { get { return true; } }
        
        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo((TItem[])array, index);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            lock (SyncRoot)
                _internalCollection.CopyTo(array, arrayIndex);
        }

        //int IList.Add(object value)
        //{
        //    throw new NotSupportedException("Use ObservableCollectionViewBase.Add() instead.");
        //}

        //public void Add(T item)
        //{
        //    throw new NotSupportedException("Use ObservableCollectionViewBase.Add() instead.");
        //}

        //public void Clear()
        //{
        //    throw new NotSupportedException("Use ObservableCollectionViewBase.Clear() instead.");
        //}

        //public bool Remove(T item)
        //{
        //    throw new NotSupportedException("Use ObservableCollectionViewBase.Remove() instead.");
        //}

        //public void Remove(object value)
        //{
        //    throw new NotSupportedException("Use ObservableCollectionViewBase.Remove() instead.");
        //}

        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        int IList.Add(object value)
        {
            return ((IList)Items).Add(value);
        }

        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        public void Add(TItem item)
        {
            Items.Add(item);
        }

        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        public void Clear()
        {
            Items.Clear();
        }

        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        public bool Remove(TItem item)
        {
            return Items.Remove(item);
        }

        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        public void Remove(object value)
        {
            ((IList)Items).Remove(value);
        }

        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        public void Insert(int index, TItem item)
        {
            throw new NotSupportedException("Use ObservableCollectionViewBase.Insert() instead.");
        }

        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        public void Insert(int index, object value)
        {
            throw new NotSupportedException("Use ObservableCollectionViewBase.Insert() instead.");
        }

        [Obsolete("Use methods on Items property instead. 9/20/2014")]
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("Use ObservableCollectionViewBase.Insert() instead.");
        }

        #endregion
    }
}