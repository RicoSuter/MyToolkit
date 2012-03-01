using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation.Collections;
namespace MyToolkit.Collections
{
	public class ObservableVector<T> : ObservableCollection<T>
	{
	}


	//public class ObservableVector<T> : ObservableCollection<T>, IObservableVector<object>
	//{
	//	public class VectorChangedEventArgs : IVectorChangedEventArgs
	//	{
	//		/// <summary>
	//		/// New instance of the args
	//		/// </summary>
	//		/// <param name="collectionChange">Type of change</param>
	//		/// <param name="index">Index at which change occurred</param>
	//		public VectorChangedEventArgs(CollectionChange collectionChange, uint index)
	//		{
	//			CollectionChange = collectionChange;
	//			Index = index;
	//		}

	//		/// <summary>
	//		/// Type of change
	//		/// </summary>
	//		public CollectionChange CollectionChange { get; private set; }

	//		/// <summary>
	//		/// Index at which change occurred
	//		/// </summary>
	//		public uint Index { get; private set; }

	//	}

	//	/// <summary>
	//	/// Raises collection changed event
	//	/// </summary>
	//	/// <param name="e">Event arguments for collection changed event</param>
	//	protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
	//	{
	//		base.OnCollectionChanged(e);
	//		switch (e.Action)
	//		{
	//			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
	//				OnVectorChanged(CollectionChange.ItemInserted, (uint)e.NewStartingIndex);
	//				break;
	//			case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
	//				OnVectorChanged(CollectionChange.Reset, (uint)e.NewStartingIndex);
	//				break;
	//			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
	//				OnVectorChanged(CollectionChange.ItemRemoved, (uint)e.OldStartingIndex);
	//				break;
	//			case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
	//				OnVectorChanged(CollectionChange.ItemChanged, (uint)e.NewStartingIndex);
	//				break;
	//			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
	//				OnVectorChanged(CollectionChange.Reset, (uint)e.NewStartingIndex);
	//				break;
	//			default:
	//				break;
	//		}
	//	}
	//	protected void OnVectorChanged(CollectionChange collectionChange, uint index)
	//	{
	//		if (VectorChanged != null)
	//			VectorChanged(this, new VectorChangedEventArgs(collectionChange, index));
	//	}

	//	public event VectorChangedEventHandler<object> VectorChanged;

	//	public int IndexOf(object item)
	//	{
	//		return base.IndexOf((T)item);
	//	}

	//	public void Insert(int index, object item)
	//	{
	//		base.InsertItem(index, (T)item);
	//	}

	//	public new object this[int index]
	//	{
	//		get
	//		{
	//			return base[index];
	//		}
	//		set
	//		{
	//			base[index] = (T)value;
	//		}
	//	}

	//	public void Add(object item)
	//	{
	//		base.Add((T)item);
	//	}

	//	public bool Contains(object item)
	//	{
	//		return base.Contains((T)item);
	//	}

	//	public void CopyTo(object[] array, int arrayIndex)
	//	{
	//		T[] newArray = new T[array.Length];
	//		for (int i = 0; i < array.Length; i++)
	//		{
	//			newArray[i] = (T)array[i];
	//		}
	//		CopyTo(newArray, arrayIndex);
	//	}

	//	public bool IsReadOnly
	//	{
	//		get { return false; }
	//	}

	//	public bool Remove(object item)
	//	{
	//		if (Contains(item))
	//		{
	//			base.Remove((T)item);
	//			return true;
	//		}
	//		return false;
	//	}

	//	public new IEnumerator<object> GetEnumerator()
	//	{
	//		return base.GetEnumerator() as IEnumerator<object>;
	//	}
	//} 
}
