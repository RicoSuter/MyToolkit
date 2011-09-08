using System;
using System.Collections.Generic;
using System.Linq; 
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MyToolkit.Utilities;

namespace MyToolkit.Collections
{
	public class FilteredObservableCollection<T> : ObservableCollection<T>
	{
		private readonly ObservableCollection<T> originalCollection;
		private readonly Func<T, bool> where;

		public FilteredObservableCollection(ObservableCollection<T> originalCollection, Func<T, bool> where)
		{
			this.originalCollection = originalCollection;
			this.where = where;

			var wref = new WeakEvent<ObservableCollection<T>, FilteredObservableCollection<T>, NotifyCollectionChangedEventHandler>(originalCollection, this);
			wref.Event = (s, e) => OnCollectionChanged(wref, s, e);
			originalCollection.CollectionChanged += wref.Event;

			UpdateList(null, null);
		}

		private static void OnCollectionChanged(
			WeakEvent<ObservableCollection<T>, FilteredObservableCollection<T>, NotifyCollectionChangedEventHandler> weakEvent, 
			object sender, NotifyCollectionChangedEventArgs args)
		{
			if (weakEvent.IsAlive)
				weakEvent.Reference.UpdateList(sender, args);
			else
				weakEvent.Target.CollectionChanged -= weakEvent.Event;
		}

		private void UpdateList(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			var list = originalCollection.Where(where).ToList();
			
			foreach (var item in this.ToArray()) // remove items
			{
				if (!list.Contains(item))
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

		public void Unload()
		{
			originalCollection.CollectionChanged -= UpdateList;
		}
	}
}
