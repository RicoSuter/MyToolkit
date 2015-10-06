//-----------------------------------------------------------------------
// <copyright file="DataManagerBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MyToolkit.Utilities;

namespace MyToolkit.Data
{
#if WP7

    internal class HashSet<T> : List<T> // TODO implement real hashset
    {
    }

#endif

    /// <summary>Provides an abstract CacheManager implementation with the ability to use a WCF service to load cacheable entities. </summary>
    public abstract class DataManagerBase<TIdentity> : CacheManager<TIdentity> // TODO: Rename to something similar to CacheManager
    {
        private readonly List<Tuple<string, TIdentity>> _loadingItems = new List<Tuple<string, TIdentity>>();
        private readonly HashSet<string> _fullLoadedTypes = new HashSet<string>();
        private readonly HashSet<string> _fullLoadingTypes = new HashSet<string>();
        private readonly List<Tuple<string, TIdentity, string, TaskCompletionSource<object>>> _waiting =
            new List<Tuple<string, TIdentity, string, TaskCompletionSource<object>>>();

        /// <summary>Loads a single item given its type and ID. </summary>
        /// <param name="type">The type of the item to load. </param>
        /// <param name="id">The ID of the item to load. </param>
        /// <param name="included">The navigation properties to include in the items (e.g. eager load using EF). </param>
        /// <returns>The loaded item. </returns>
        protected abstract Task<object> GetItemAsync(string type, TIdentity id, string[] included);

        /// <summary>Loads all items of a given type. </summary>
        /// <param name="type">The type of the items to load. </param>
        /// <param name="included">The navigation properties to include in the item (e.g. eager load using EF). </param>
        /// <returns>The loaded items. </returns>
        protected abstract Task<IEnumerable<object>> GetAllItemsAsync(string type, string[] included);

        /// <summary>Loads a collection navigation property of a given item. </summary>
        /// <param name="type">The item type. </param>
        /// <param name="id">The item ID. </param>
        /// <param name="propertyName">The collection navigation property of the specified item. </param>
        /// <param name="included">The navigation properties to include in the items (e.g. eager load using EF). </param>
        /// <returns>The collection of the given collection navigation property. </returns>
        protected abstract Task<IEnumerable<object>> GetCollectionPropertyForItemAsync(
            string type, TIdentity id, string propertyName, string[] included);

        /// <summary>Loads a single item. </summary>
        /// <typeparam name="T">The type of the item. </typeparam>
        /// <param name="id">The ID of the item. </param>
        /// <param name="reload">Specifies whether to use the cached version (if available) or always reload the item. </param>
        /// <param name="included">The navigation properties to include in the items (e.g. eager load using EF). </param>
        /// <returns>The item. </returns>
        public Task<T> GetItemAsync<T>(TIdentity id, bool reload = true, params Expression<Func<T, object>>[] included)
        {
            return GetItemAsync<T>(id, reload, included.Select(ExpressionUtilities.GetPropertyName).ToArray());
        }

        /// <summary>Loads a single item. </summary>
        /// <typeparam name="T">The type of the item. </typeparam>
        /// <param name="id">The ID of the item. </param>
        /// <param name="reload">Specifies whether to use the cached version (if available) or always reload the item. </param>
        /// <param name="included">The navigation properties to include in the items (e.g. eager load using EF). </param>
        /// <returns>The item. </returns>
        public async Task<T> GetItemAsync<T>(TIdentity id, bool reload = true, params string[] included)
        {
            return (T)await GetItemAsync(GetBaseType(typeof(T)).Name, id, reload, included);
        }

        private async Task<IEntity<TIdentity>> GetItemAsync(string type, TIdentity id, bool reload, string[] included)
        {
            var item = GetItem(type, id);
            if (item != null && !reload)
                return item;

#if !LEGACY
            var loadingProperty = item != null ? item.GetType().GetRuntimeProperty("IsLoading") : null;
#else
            var loadingProperty = item != null ? item.GetType().GetProperty("IsLoading") : null;
#endif
            if (loadingProperty != null)
                loadingProperty.SetValue(item, true, null);

            try
            {
                if (_loadingItems.Any(t => t.Item1 == type && Equals(t.Item2, id)))
                {
                    await WaitUntilCompleted(type, id, null);
                    return GetItem(type, id);
                }

                var tuple = new Tuple<string, TIdentity>(type, id);
                _loadingItems.Add(tuple);
                try
                {
                    var result = await GetItemAsync(type, id, included ?? new string[] { });
                    if (result != null)
                        return AddItem((IEntity<TIdentity>)result);
                }
                catch (Exception ex)
                {
                    CompleteWaitingWithException(type, default(TIdentity), null, ex);
                }
                finally
                {
                    CompleteWaiting(type, default(TIdentity), null);
                    _loadingItems.Remove(tuple);
                }
                return null;
            }
            finally
            {
                if (loadingProperty != null)
                    loadingProperty.SetValue(item, false, null);
            }
        }

        /// <summary>Loads all items of a given type. </summary>
        /// <typeparam name="T">The type of the item. </typeparam>
        /// <param name="reload">Specifies whether to use the cached version (if available) or always reload the item. </param>
        /// <param name="included">The navigation properties to include in the item (e.g. eager load using EF). </param>
        /// <returns>The loaded items. </returns>
        public Task<IEnumerable<T>> GetAllItemsAsync<T>(bool reload = true, params Expression<Func<T, object>>[] included)
        {
            return GetAllItemsAsync<T>(reload, included.Select(ExpressionUtilities.GetPropertyName).ToArray());
        }

        /// <summary>Loads all items of a given type. </summary>
        /// <typeparam name="T">The type of the item. </typeparam>
        /// <param name="reload">Specifies whether to use the cached version (if available) or always reload the item. </param>
        /// <param name="included">The navigation properties to include in the item (e.g. eager load using EF). </param>
        /// <returns>The loaded items. </returns>
        public async Task<IEnumerable<T>> GetAllItemsAsync<T>(bool reload = true, params string[] included)
        {
            return (await GetAllItemsAsync(GetBaseType(typeof(T)).Name, reload, included))
                .OfType<T>().ToList();
        }

        private async Task<IEnumerable<IEntity<TIdentity>>> GetAllItemsAsync(string type, bool reload, params string[] included)
        {
            if (!reload && _fullLoadedTypes.Contains(type))
                return List[type].Select(p => p.Value).ToList();

            if (_fullLoadingTypes.Contains(type)) // already loading
            {
                await WaitUntilCompleted(type, default(TIdentity), null);
                return List[type].Select(p => p.Value).ToList();
            }

            _fullLoadingTypes.Add(type);
            try
            {
                var items = (await GetAllItemsAsync(type, included)).OfType<IEntity<TIdentity>>();
                _fullLoadedTypes.Add(type);

                if (List.ContainsKey(type)) // remove deleted entities from cache
                {
                    var toRemove = List[type].Where(p => items.All(i => !Equals(i.Id, p.Value.Id))).ToList();
                    foreach (var r in toRemove)
                        List[type].Remove(r.Key);
                }
                return AddItems(items);
            }
            catch (Exception ex)
            {
                CompleteWaitingWithException(type, default(TIdentity), null, ex);
            }
            finally
            {
                CompleteWaiting(type, default(TIdentity), null);
                _fullLoadingTypes.Remove(type);
            }
            return null;
        }

        /// <summary>Loads a property for a given item. </summary>
        /// <typeparam name="T">The type of the item. </typeparam>
        /// <typeparam name="TProperty">The type of the property. </typeparam>
        /// <param name="item">The item. </param>
        /// <param name="propertyName">The name of the property to load. </param>
        /// <param name="reload">Specifies whether to use the cached version (if available) or always reload the item. </param>
        /// <param name="included">The navigation properties to include in the item (e.g. eager load using EF). </param>
        /// <returns>The task. </returns>
        public Task LoadPropertyForItemAsync<T, TProperty>(T item, Expression<Func<T, object>> propertyName, bool reload = true, params string[] included)
            where T : IEntity<TIdentity>
            where TProperty : IEntity<TIdentity>
        {
            return LoadPropertyForItemAsync<T, TProperty>(item, ExpressionUtilities.GetPropertyName(propertyName), reload, included);
        }

        /// <summary>Loads a property for a given item. </summary>
        /// <typeparam name="T">The type of the item. </typeparam>
        /// <typeparam name="TProperty">The type of the property. </typeparam>
        /// <param name="item">The item. </param>
        /// <param name="propertyName">The name of the property to load. </param>
        /// <param name="reload">Specifies whether to use the cached version (if available) or always reload the item. </param>
        /// <param name="included">The navigation properties to include in the item (e.g. eager load using EF). </param>
        /// <returns>The task. </returns>
        public async Task LoadPropertyForItemAsync<T, TProperty>(T item, string propertyName, bool reload = true, params string[] included)
            where T : IEntity<TIdentity>
            where TProperty : IEntity<TIdentity>
        {
            var info = item.GetType();
            var baseType = GetBaseType(typeof(T));
            var itemType = baseType.Name;

#if !LEGACY
            var property = info.GetRuntimeProperty(propertyName);
            var isCollectionProperty = typeof(IList).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo());
#else
            var property = info.GetProperty(propertyName);
            var isCollectionProperty = typeof(IList).IsAssignableFrom(property.PropertyType);
#endif

            // check loaded
            TIdentity foreignId = default(TIdentity);
            if (!reload)
            {
                var currentValue = property.GetValue(item, null);
                if (isCollectionProperty)
                {
                    if (currentValue != null)
                        return;
                }
                else
                {
#if !LEGACY
                    var idProperty = info.GetRuntimeProperty(propertyName + "Id");
#else
                    var idProperty = info.GetProperty(propertyName + "Id");
#endif
                    if (idProperty.PropertyType == typeof(int?))
                    {
                        var foreignIdObject = idProperty.GetValue(item, null);

                        // sync navigation proprety with foreign id
                        if (foreignIdObject == null)
                        {
                            if (currentValue != null)
                                property.SetValue(item, null, null);
                            return;
                        }
                        else
                        {
                            foreignId = (TIdentity)foreignIdObject;
                            if (currentValue != null && Equals(((IEntity<TIdentity>)currentValue).Id, foreignId))
                                return;
                            property.SetValue(item, null, null);
                        }
                    }
                    else
                    {
                        var id = (int)idProperty.GetValue(item, null);
                        if (currentValue != null && Equals(((IEntity<TIdentity>)currentValue).Id, id))
                            return;
                    }
                }
            }

            // check IsLoading (already loading)
#if !LEGACY
            var loadingProperty = info.GetRuntimeProperty("Is" + propertyName + "Loading");
#else
            var loadingProperty = info.GetProperty("Is" + propertyName + "Loading");
#endif
            if (loadingProperty != null)
            {
                var isLoading = (bool)loadingProperty.GetValue(item, null);
                if (isLoading)
                {
                    await WaitUntilCompleted(itemType, item.Id, propertyName);
                    return;
                }
            }
            else
                Debug.WriteLine("DataManagerBase - Warning: No 'Is" + propertyName + "Loading' property found on '" + info.Name + "'");

            // set IsLoading = true
            if (loadingProperty != null)
                loadingProperty.SetValue(item, true, null);

            // call web service
            try
            {
                if (isCollectionProperty)
                {
                    var result = await GetCollectionPropertyForItemAsync(baseType.Name, item.Id, propertyName, included ?? new string[] { });
                    if (result != null)
                    {
                        var items = AddItems(result).OfType<TProperty>();
                        property.SetValue(item, new ObservableCollection<TProperty>(items), null);
                    }
                }
                else
                {
                    var result = await GetItemAsync(GetBaseType(property.PropertyType).Name, foreignId, included ?? new string[] { });
                    if (result != null)
                        property.SetValue(item, AddItem((TProperty)result), null);
                }
            }
            catch (Exception ex)
            {
                CompleteWaitingWithException(itemType, item.Id, propertyName, ex);
            }
            finally
            {
                CompleteWaiting(itemType, item.Id, propertyName);
                if (loadingProperty != null)
                    loadingProperty.SetValue(item, false, null);
            }
        }

        private Task WaitUntilCompleted(string type, TIdentity id, string propertyName)
        {
            var task = new TaskCompletionSource<object>();
            _waiting.Add(new Tuple<string, TIdentity, string, TaskCompletionSource<object>>(type, id, propertyName, task));
            return task.Task;
        }

        private void CompleteWaiting(string type, TIdentity id, string propertyName)
        {
            var finished = _waiting.Where(t => t.Item1 == type && Equals(t.Item2, id) && t.Item3 == propertyName).ToArray();
            foreach (var f in finished)
            {
                f.Item4.SetResult(null);
                _waiting.Remove(f);
            }
        }

        private void CompleteWaitingWithException(string type, TIdentity id, string propertyName, Exception ex)
        {
            var finished = _waiting.Where(t => t.Item1 == type && Equals(t.Item2, id) && t.Item3 == propertyName).ToArray();
            foreach (var f in finished)
            {
                f.Item4.SetException(ex);
                _waiting.Remove(f);
            }
        }
    }
}