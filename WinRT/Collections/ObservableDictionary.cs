using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MyToolkit.Collections
{
	namespace System.Collections.ObjectModel
	{
		public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
		{
			private IDictionary<TKey, TValue> dictionary;
			protected IDictionary<TKey, TValue> Dictionary
			{
				get { return dictionary; }
			}

			public ObservableDictionary()
			{
				dictionary = new Dictionary<TKey, TValue>();
			}
			public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
			{
				this.dictionary = new Dictionary<TKey, TValue>(dictionary);
			}
			public ObservableDictionary(IEqualityComparer<TKey> comparer)
			{
				dictionary = new Dictionary<TKey, TValue>(comparer);
			}
			public ObservableDictionary(int capacity)
			{
				dictionary = new Dictionary<TKey, TValue>(capacity);
			}
			public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
			{
				this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
			}
			public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
			{
				dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
			}

			public void AddRange(IDictionary<TKey, TValue> items)
			{
				if (items == null) 
					throw new ArgumentNullException("items");

				if (items.Count > 0)
				{
					if (Dictionary.Count > 0)
					{
						if (items.Keys.Any((k) => Dictionary.ContainsKey(k)))
							throw new ArgumentException("An item with the same key has already been added.");
						
						foreach (var item in items) Dictionary.Add(item);
					}
					else
						dictionary = new Dictionary<TKey, TValue>(items);

					OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
				}
			}

			protected virtual void Insert(TKey key, TValue value, bool add)
			{
				if (key == null) 
					throw new ArgumentNullException("key");

				TValue item;
				if (Dictionary.TryGetValue(key, out item))
				{
					if (add) 
						throw new ArgumentException("An item with the same key has already been added.");

					if (Equals(item, value)) 
						return;

					Dictionary[key] = value;
					OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, item));
				}
				else
				{
					Dictionary[key] = value;
					OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
				}
			}

			private void OnPropertyChanged()
			{
				OnPropertyChanged("Count");
				OnPropertyChanged("Item[]");
				OnPropertyChanged("Keys");
				OnPropertyChanged("Values");
			}

			protected virtual void OnPropertyChanged(string propertyName)
			{
				var copy = PropertyChanged;
				if (copy != null)
					copy(this, new PropertyChangedEventArgs(propertyName));
			}

			protected void OnCollectionChanged()
			{
				OnPropertyChanged();
				var copy = CollectionChanged;
				if (copy != null)
					copy(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}

			protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
			{
				OnPropertyChanged();
				var copy = CollectionChanged;
				if (copy != null)
					copy(this, new NotifyCollectionChangedEventArgs(action, changedItem));
			}

			protected void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
			{
				OnPropertyChanged();
				var copy = CollectionChanged;
				if (copy != null)
					copy(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
			}

			protected void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
			{
				OnPropertyChanged();
				var copy = CollectionChanged;
				if (copy != null)
					copy(this, new NotifyCollectionChangedEventArgs(action, newItems));
			}

			#region IDictionary<TKey,TValue> interface

			public void Add(TKey key, TValue value)
			{
				Insert(key, value, true);
			}

			public bool ContainsKey(TKey key)
			{
				return Dictionary.ContainsKey(key);
			}

			public ICollection<TKey> Keys
			{
				get { return Dictionary.Keys; }
			}

			public virtual bool Remove(TKey key)
			{
				if (key == null)
					throw new ArgumentNullException("key");

				TValue value;
				Dictionary.TryGetValue(key, out value);

				var removed = Dictionary.Remove(key);
				if (removed)
					OnCollectionChanged();
				//OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value));
				return removed;
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				return Dictionary.TryGetValue(key, out value);
			}

			public ICollection<TValue> Values
			{
				get { return Dictionary.Values; }
			}

			public TValue this[TKey key]
			{
				get { return Dictionary[key]; }
				set { Insert(key, value, false); }
			}

			#endregion

			#region ICollection<KeyValuePair<TKey,TValue>> interface

			public void Add(KeyValuePair<TKey, TValue> item)
			{
				Insert(item.Key, item.Value, true);
			}

			public void Clear()
			{
				if (Dictionary.Count > 0)
				{
					Dictionary.Clear();
					OnCollectionChanged();
				}
			}

			public bool Contains(KeyValuePair<TKey, TValue> item)
			{
				return Dictionary.Contains(item);
			}

			public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
			{
				Dictionary.CopyTo(array, arrayIndex);
			}

			public int Count
			{
				get { return Dictionary.Count; }
			}

			public bool IsReadOnly
			{
				get { return Dictionary.IsReadOnly; }
			}

			public bool Remove(KeyValuePair<TKey, TValue> item)
			{
				return Remove(item.Key);
			}

			#endregion

			#region IEnumerable<KeyValuePair<TKey,TValue>> interface

			public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			{
				return Dictionary.GetEnumerator();
			}

			#endregion

			#region IEnumerable interface

			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable)Dictionary).GetEnumerator();
			}

			#endregion

			#region INotifyCollectionChanged interface

			public event NotifyCollectionChangedEventHandler CollectionChanged;

			#endregion

			#region INotifyPropertyChanged interface

			public event PropertyChangedEventHandler PropertyChanged;

			#endregion
		}
	}   
}
