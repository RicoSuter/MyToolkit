//-----------------------------------------------------------------------
// <copyright file="MtObservableCollection.cs" company="MyToolkit">
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

namespace MyToolkit.Collections
{
    /// <summary>Provides a safe collection changed event which always provides the added 
    /// and removed items, some more events and more range methods. </summary>
    /// <typeparam name="T"></typeparam>
    public class MtObservableCollection<T> : ObservableCollection<T>
    {
        private List<T> _oldCollection;
        private event EventHandler<MtNotifyCollectionChangedEventArgs<T>> _extendedCollectionChanged;

        /// <summary>Initializes a new instance of the <see cref="MtObservableCollection{T}"/> class.</summary>
        public MtObservableCollection() { }

        /// <summary>Initializes a new instance of the <see cref="MtObservableCollection{T}"/> class.</summary>
        /// <param name="collection">The collection.</param>
        public MtObservableCollection(IEnumerable<T> collection) : base(collection) { }

        /// <summary>Gets or sets a value indicating whether to provide the previous collection in the extended collection changed event. 
        /// Enabling this feature may have a performance impact as for each collection changed event a copy of the collection gets created. </summary>
        public bool ProvideOldCollection { get; set; }

        /// <summary>Occurs when a property value changes. 
        /// This is the same event as on the <see cref="ObservableCollection{T}"/> except that it is public. </summary>
        public new event PropertyChangedEventHandler PropertyChanged
        {
            add { base.PropertyChanged += value; }
            remove { base.PropertyChanged -= value; }
        }

        /// <summary>Adds multiple items to the collection. </summary>
        /// <param name="collection">The items to add. </param>
        /// <exception cref="ArgumentNullException">The value of 'collection' cannot be null. </exception>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
                Items.Add(item);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
#if LEGACY
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
#else
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection.ToList()));
#endif
        }

        /// <summary>Removes multiple items from the collection. </summary>
        /// <param name="collection">The items to remove. </param>
        /// <exception cref="ArgumentNullException">The value of 'collection' cannot be null. </exception>
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection.ToList())
                Items.Remove(item);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
#if LEGACY
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
#else
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, collection.ToList()));
#endif
        }

        /// <summary>Resets the whole collection with a given list. </summary>
        /// <param name="collection">The collection. </param>
        /// <exception cref="ArgumentNullException">The value of 'collection' cannot be null. </exception>
        public void Initialize(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            Items.Clear();
            foreach (var i in collection)
                Items.Add(i);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>Collection changed event with safe/always correct added items and removed items list. </summary>
        public event EventHandler<MtNotifyCollectionChangedEventArgs<T>> ExtendedCollectionChanged
        {
            add
            {
                lock (this)
                {
                    if (_extendedCollectionChanged == null)
                        _oldCollection = new List<T>(this);
                    _extendedCollectionChanged += value;
                }
            }
            remove
            {
                lock (this)
                {
                    _extendedCollectionChanged -= value;
                    if (_extendedCollectionChanged == null)
                        _oldCollection = null;
                }
            }
        }

        /// <summary>Raises the System.Collections.ObjectModel.ObservableCollection{T}.CollectionChanged event with the provided arguments. </summary>
        /// <param name="e">Arguments of the event being raised. </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            var copy = _extendedCollectionChanged;
            if (copy != null)
            {
                var oldCollection = ProvideOldCollection ? _oldCollection.ToList() : null;

                var addedItems = new List<T>();
                foreach (var item in this.Where(x => !_oldCollection.Contains(x))) // new items
                {
                    addedItems.Add(item);
                    _oldCollection.Add(item);
                }

                var removedItems = new List<T>();
                foreach (var item in _oldCollection.Where(x => !Contains(x)).ToArray()) // deleted items
                {
                    removedItems.Add(item);
                    _oldCollection.Remove(item);
                }

                copy(this, new MtNotifyCollectionChangedEventArgs<T>(addedItems, removedItems, oldCollection));
            }
        }
    }

    [Obsolete("Use MtObservableCollection<T> instead. 11/29/2014")]
    public class ExtendedObservableCollection<T> : MtObservableCollection<T>
    {
        public ExtendedObservableCollection()
        {
        }

        public ExtendedObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }
    }
}