//-----------------------------------------------------------------------
// <copyright file="ObservableCollectionObserver.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyToolkit.Collections
{
	/// <summary>Provides events to react on changes of an ObservableCollection and its elements. </summary>
	/// <typeparam name="T">The item type. </typeparam>
	public class ObservableCollectionObserver<T> where T : INotifyPropertyChanged
	{
        private ObservableCollection<T> _list;
        private readonly List<T> _registeredItems = new List<T>();
        
        /// <summary>Occurs when the collection changes. </summary>
		public event EventHandler<ExtendedNotifyCollectionChangedEventArgs<T>> CollectionChanged;

		/// <summary>Occurs when an element of the collection changes. </summary>
		public event PropertyChangedEventHandler ItemChanged;

		public ObservableCollectionObserver(ObservableCollection<T> list = null)
		{
			Initialize(list);
		}

		/// <summary>Sets the observed collection. </summary>
		/// <param name="collection">The collection. </param>
		public void Initialize(ObservableCollection<T> collection)
		{
			if (collection == _list)
				return;

			// unregister all events from old list
			if (_list != null)
			{
				_list.CollectionChanged -= OnCollectionChanged;
				foreach (var i in _registeredItems)
					i.PropertyChanged -= OnPropertyChanged;
				_registeredItems.Clear();
			}

			// register new events
			_list = collection;
			if (_list != null)
			{
				_list.CollectionChanged += OnCollectionChanged;
				foreach (var i in _list)
				{
					_registeredItems.Add(i);
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

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			// get new items
			var addedItems = new List<T>();
			foreach (var i in _list.Where(x => !_registeredItems.Contains(x))) 
			{
				addedItems.Add(i);
				_registeredItems.Add(i);
				i.PropertyChanged += OnPropertyChanged;
			}

			// get deleted items
			var removedItems = new List<T>();
			foreach (var i in _registeredItems.Where(x => !_list.Contains(x)).ToArray()) 
			{
				removedItems.Add(i);
				_registeredItems.Remove(i);
				i.PropertyChanged -= OnPropertyChanged;
			}

			var copy = CollectionChanged;
			if (copy != null)
                copy(sender, new ExtendedNotifyCollectionChangedEventArgs<T>(addedItems, removedItems, null));
		}
	}
}
