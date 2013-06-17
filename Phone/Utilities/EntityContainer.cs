using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyToolkit.Collections;
using MyToolkit.Messaging;
using MyToolkit.Storage;

#if WP7 && !LIGHT
using MyToolkit.Environment;
using MyToolkit.Messaging;
#endif

namespace MyToolkit.Utilities
{
	public interface IEntity
	{
		int Id { get; }
	}

	public class EntityContainer<T>
		where T : class, IEntity
	{
		private readonly ExtendedObservableCollection<T> collection = new ExtendedObservableCollection<T>();
		public ExtendedObservableCollection<T> Collection { get { return collection; } }

		public T Get(string idAsString)
		{
			var id = int.Parse(idAsString);
			return collection.SingleOrDefault(n => n.Id == id);
		}

		public T Get(long id)
		{
			return collection.SingleOrDefault(n => n.Id == id);
		}

		public void Initialize(IEnumerable<T> items)
		{
			Collection.Initialize(items);
		}

		public void Clear()
		{
			Collection.Clear();
		}

		public void AddRange(IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				var c = Get(item.Id);
				if (c != null)
					collection.Remove(c);
			}
			collection.AddRange(items);
		}

		public void Insert(int i, T item)
		{
			var c = Get(item.Id);
			if (c != null)
				collection.Remove(c);
			collection.Insert(i, item);
		}

		public void Add(T item)
		{
			var c = Get(item.Id);
			if (c != null)
				collection.Remove(c);
			collection.Add(item);
		}

		public void Remove(T item)
		{
			var c = Get(item.Id);
			if (c != null)
			{
				if (c is IDisposable)
					((IDisposable)c).Dispose();

				collection.Remove(c);
			}
			else if (item is IDisposable)
				((IDisposable)item).Dispose();
		}

		public void SaveToSettings(string key)
		{
			ApplicationSettings.SetSetting(key, collection.ToList());
		}

		public void LoadFromSettings(string key, List<T> initialList = null)
		{
			if (initialList == null)
				initialList = new List<T>();

			collection.Clear();
			foreach (var a in ApplicationSettings.GetSetting(key, initialList))
				collection.Add(a);
		}

		public virtual int GenerateIdentity()
		{
			lock (this)
				return IdentityGenerator.Generate(i => collection.Any(c => c.Id == i));
		}

#if WP7
		/// <summary>
		/// Call this method in OnBackKeyPress
		/// </summary>
		/// <param name="item"></param>
		/// <param name="msg"></param>
		/// <returns>set e.Cancel to result</returns>
		public bool TrySave(T item, TextMessage msg)
		{
			if (msg != null)
			{
				Messenger.Send(msg);
				if (msg.Result == TextMessage.MessageResult.OK)
					Remove(item);
				else
					return true;
			}
			else
				Add(item);
			return false;
		}
#endif
	}
}
