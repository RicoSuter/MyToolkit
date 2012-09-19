using System.Collections.Generic;

namespace MyToolkit.Collections
{
	public class ExtendedNotifyCollectionChangedEventArgs<T>
	{
		public List<T> AddedItems { get; set; }
		public List<T> RemovedItems { get; set; }
	}
}