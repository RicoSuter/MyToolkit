using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyToolkit.Collections
{
	public class ExtendedObservableCollection<T> : ObservableCollection<T>
	{
		public new event PropertyChangedEventHandler PropertyChanged
		{
			add { base.PropertyChanged += value; }
			remove { base.PropertyChanged -= value; }
		}
		
		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null) 
				throw new ArgumentNullException("collection");

			var list = collection.ToList();
			foreach (var i in list) 
				Items.Add(i);

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			//var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add);
			//foreach (var i in list)
			//    args.NewItems.Add(i);
			//OnCollectionChanged(args);
		}

		public void RemoveRange(IEnumerable<T> collection)
		{
			if (collection == null) 
				throw new ArgumentNullException("collection");

			var list = collection.ToList();
			foreach (var i in list) 
				Items.Remove(i);

			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			//var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove);
			//foreach (var i in list)
			//    args.OldItems.Add(i);
			//OnCollectionChanged(args);
		}

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
	}
}