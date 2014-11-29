using System;
using System.Collections.Generic;
using System.Linq;
using MyToolkit.Collections;
using MyToolkit.Storage;

namespace MyToolkit.Utilities
{
    // TODO: Improve this class

    public class EntityContainer<TEntity, TIdentity> where TEntity : class, IEntity<TIdentity>
	{
		private readonly MtObservableCollection<TEntity> _collection = new MtObservableCollection<TEntity>();

		public MtObservableCollection<TEntity> Collection 
        { 
            get { return _collection; } 
        }

        public TEntity Get(TIdentity id)
		{
			return _collection.SingleOrDefault(n => Equals(n.Id, id));
		}

		public void Initialize(IEnumerable<TEntity> items)
		{
			Collection.Initialize(items);
		}

		public void Clear()
		{
			Collection.Clear();
		}

		public void AddRange(IEnumerable<TEntity> items)
		{
			foreach (var item in items)
			{
				var c = Get(item.Id);
				if (c != null)
					_collection.Remove(c);
			}

			_collection.AddRange(items);
		}

		public void Insert(int i, TEntity item)
		{
			var c = Get(item.Id);
			if (c != null)
				_collection.Remove(c);
			_collection.Insert(i, item);
		}

		public void Add(TEntity item)
		{
			var c = Get(item.Id);
			if (c != null)
				_collection.Remove(c);
			_collection.Add(item);
		}

		public void Remove(TEntity item)
		{
			var c = Get(item.Id);
			if (c != null)
			{
				if (c is IDisposable)
					((IDisposable)c).Dispose();

				_collection.Remove(c);
			}
			else if (item is IDisposable)
				((IDisposable)item).Dispose();
		}

		public string GenerateIdentity()
		{
			return Guid.NewGuid().ToString();
		}

        [Obsolete("Use methods from ApplicationSettingsEx class instead. 5/17/2014")]
        public void SaveToSettings(string key) 
		{
			ApplicationSettings.SetSetting(key, Collection.ToList());
		}

        [Obsolete("Use methods from ApplicationSettingsEx class instead. 5/17/2014")]
        public void LoadFromSettings(string key, List<TEntity> initialList = null) 
		{
			if (initialList == null)
				initialList = new List<TEntity>();
			Collection.Initialize(ApplicationSettings.GetSetting(key, initialList));
		}
	}
}
