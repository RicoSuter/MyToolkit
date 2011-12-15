using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyToolkit.Phone;

namespace MyToolkit.Utilities
{
	public interface IEntity
	{
		int ID { get; }
	}

	public class EntityContainer<T>
		where T : class, IEntity
	{
		private readonly ObservableCollection<T> collection = new ObservableCollection<T>();
		public ObservableCollection<T> Collection { get { return collection; } }

		public T Get(string idAsString)
		{
			var id = int.Parse(idAsString);
			return collection.SingleOrDefault(n => n.ID == id);
		}

		public T Get(int id)
		{
			return collection.SingleOrDefault(n => n.ID == id);
		}

		public void Initialize(IEnumerable<T> items)
		{
			Collection.Clear();
			foreach (var i in items)
				Collection.Add(i);
		}

		public void Clear()
		{
			Collection.Clear();
		}
		
		public void Add(T item)
		{
			var c = Get(item.ID);
			if (c != null)
				collection.Remove(c);
			collection.Add(item);
		}

		public void Remove(T item)
		{
			var c = Get(item.ID);
			if (c != null)
				collection.Remove(c);
		}

		public void SaveToSettings(string key)
		{
			Settings.SetSetting(key, collection.ToList());
		}

		public void LoadFromSettings(string key)
		{
			collection.Clear();
			foreach (var a in Settings.GetSetting(key, new List<T>()))
				collection.Add(a);
		}

		public virtual int GenerateIdentity()
		{
			return IdentityGenerator.Generate(i => collection.Any(c => c.ID == i));
		}
	}
}
