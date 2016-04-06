using System;
using System.Collections.Generic;
using System.Linq;
using MyToolkit.Collections;
using MyToolkit.Storage;

namespace MyToolkit.Utilities
{
    // TODO: Improve this class

    /// <summary>A container to manage entities of a given type.</summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TIdentity">The type of the identity.</typeparam>
    public class EntityContainer<TEntity, TIdentity> where TEntity : class, IEntity<TIdentity>
    {
        private MtObservableCollection<TEntity> _collection = new MtObservableCollection<TEntity>();
        
        /// <summary>Gets the entity collection.</summary>
        public MtObservableCollection<TEntity> Collection
        {
            get { return _collection; }
        }

        /// <summary>Gets an entity by ID.</summary>
        /// <returns>The entity.</returns>
        public TEntity Get(TIdentity id)
        {
            return Collection.SingleOrDefault(n => Equals(n.Id, id));
        }

        /// <summary>Initializes the container with some entities.</summary>
        /// <param name="items">The entities.</param>
        public void Initialize(IEnumerable<TEntity> items)
        {
            Collection.Initialize(items);
        }

        /// <summary>Removes all entities.</summary>
        public void Clear()
        {
            Collection.Clear();
        }

        /// <summary>Adds multiple entities.</summary>
        /// <param name="items">The entities.</param>
        public void AddRange(IList<TEntity> items)
        {
            foreach (var c in items.Select(item => Get(item.Id)).Where(c => c != null))
                Collection.Remove(c);

            Collection.AddRange(items);
        }

        /// <summary>Inserts an entity.</summary>
        /// <param name="position">The position to insert the entity.</param>
        /// <param name="item">The entity.</param>
        public void Insert(int position, TEntity item)
        {
            var entity = Get(item.Id);
            if (entity != null)
                Collection.Remove(entity);
            Collection.Insert(position, item);
        }

        /// <summary>Adds an entity at the end of the collection.</summary>
        /// <param name="item">The entity.</param>
        public void Add(TEntity item)
        {
            var c = Get(item.Id);
            if (c != null)
                Collection.Remove(c);
            Collection.Add(item);
        }

        /// <summary>Removes an entity from the collection.</summary>
        /// <param name="item">The entity.</param>
        public void Remove(TEntity item)
        {
            var c = Get(item.Id);
            if (c != null)
            {
                if (c is IDisposable)
                    ((IDisposable)c).Dispose();

                Collection.Remove(c);
            }
            else if (item is IDisposable)
                ((IDisposable)item).Dispose();
        }

        /// <summary>Generates a new identity string.</summary>
        /// <returns>The ID string.</returns>
        public string GenerateIdentity()
        {
            return Guid.NewGuid().ToString();
        }

#pragma warning disable 1591

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

#pragma warning restore 1591

    }
}
