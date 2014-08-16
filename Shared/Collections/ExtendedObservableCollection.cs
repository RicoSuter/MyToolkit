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
		public ExtendedObservableCollection() {}
 		public ExtendedObservableCollection(IEnumerable<T> collection) : base(collection) {} 

		public new event PropertyChangedEventHandler PropertyChanged
		{
			add { base.PropertyChanged += value; }
			remove { base.PropertyChanged -= value; }
		}
		
		public void AddRange(IEnumerable<T> collection)
		{
			if (collection == null) 
				throw new ArgumentNullException("collection");

			foreach (var i in collection) 
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

		private List<T> previousList = null;
		private event EventHandler<ExtendedNotifyCollectionChangedEventArgs<T>> extendedCollectionChanged;
		public event EventHandler<ExtendedNotifyCollectionChangedEventArgs<T>> ExtendedCollectionChanged
		{
			add
			{
				extendedCollectionChanged += value;
				if (previousList == null)
					previousList = new List<T>(this);
			}
			remove { extendedCollectionChanged -= value; }
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			var copy = extendedCollectionChanged;
			if (copy != null)
			{
				var addedItems = new List<T>();
				foreach (var i in this.Where(x => !previousList.Contains(x))) // new itemss
				{
					addedItems.Add(i);
					previousList.Add(i);
				}

				var removedItems = new List<T>();
				foreach (var i in previousList.Where(x => !Contains(x)).ToArray()) // deleted items
				{
					removedItems.Add(i);
					previousList.Remove(i);
				}

				copy(this, new ExtendedNotifyCollectionChangedEventArgs<T>
				{
					AddedItems = addedItems, 
					RemovedItems = removedItems
				});
			}
		}
	}
}