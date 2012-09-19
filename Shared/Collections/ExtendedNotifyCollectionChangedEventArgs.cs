using System;
using System.Collections.Generic;

namespace MyToolkit.Collections
{
	public class ExtendedNotifyCollectionChangedEventArgs<T> : EventArgs
	{
		public List<T> AddedItems { get; set; }
		public List<T> RemovedItems { get; set; }
	}
}