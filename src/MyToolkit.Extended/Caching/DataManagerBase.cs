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

namespace MyToolkit.Caching
{
    public abstract class DataManagerBase : CacheManager
    {
        protected abstract Task<object> GetItemAsync(string type, int id, string[] included);

        protected abstract Task<IEnumerable<object>> GetAllItemsAsync(string type, string[] included);

        protected abstract Task<IEnumerable<object>> GetCollectionPropertyForItemAsync(
            string type, int id, string propertyName, string[] included);

        public Task<T> GetItemAsync<T>(int id, bool reload = true, params Expression<Func<T, object>>[] included)
        {
            var list = new List<string>();
            foreach (var inc in included)
                list.Add(ExpressionHelper.GetName(inc));
            return GetItemAsync<T>(id, reload, list.ToArray());
        }

        public async Task<T> GetItemAsync<T>(int id, bool reload = true, params string[] included)
        {
            return (T)await GetItemAsync(GetBaseType(typeof(T)).Name, id, reload, included);
        }

        private readonly List<Tuple<string, int>> loadingItems = new List<Tuple<string, int>>();
        private async Task<ICacheable> GetItemAsync(string type, int id, bool reload, string[] included)
        {
            var item = GetItem(type, id);
            if (item != null && !reload)
                return item;

            var loadingProperty = item != null ? item.GetType().GetRuntimeProperty("IsLoading") : null;
            if (loadingProperty != null)
                loadingProperty.SetValue(item, true, null);

            try
            {
                if (loadingItems.Any(t => t.Item1 == type && t.Item2 == id))
                {
                    await WaitUntilCompleted(type, id, null);
                    return GetItem(type, id);
                }

                var tuple = new Tuple<string, int>(type, id);
                loadingItems.Add(tuple);
                try
                {
                    var result = await GetItemAsync(type, id, included ?? new string[] { });
                    if (result != null)
                        return AddItem((ICacheable)result);
                }
                catch (Exception ex)
                {
                    CompleteWaitingWithException(type, 0, null, ex);
                }
                finally
                {
                    CompleteWaiting(type, 0, null);
                    loadingItems.Remove(tuple);
                }
                return null;
            }
            finally 
            {
                if (loadingProperty != null)
                    loadingProperty.SetValue(item, false, null);
            }
        }

        public Task<IEnumerable<T>> GetAllItemsAsync<T>(bool reload = true, params Expression<Func<T, object>>[] included)
        {
            var list = new List<string>();
            foreach (var inc in included)
                list.Add(ExpressionHelper.GetName(inc));
            return GetAllItemsAsync<T>(reload, list.ToArray());
        }

        public async Task<IEnumerable<T>> GetAllItemsAsync<T>(bool reload = true, params string[] included)
        {
            return (await GetAllItemsAsync(GetBaseType(typeof(T)).Name, reload, included))
                .OfType<T>().ToList();
        }

        private readonly HashSet<string> fullLoadedTypes = new HashSet<string>();
        private readonly HashSet<string> fullLoadingTypes = new HashSet<string>();
        private async Task<IEnumerable<ICacheable>> GetAllItemsAsync(string type, bool reload, params string[] included)
        {
            if (!reload && fullLoadedTypes.Contains(type))
                return Data[type].Select(p => p.Value).ToList();

            if (fullLoadingTypes.Contains(type)) // already loading
            {
                await WaitUntilCompleted(type, 0, null);
                return Data[type].Select(p => p.Value).ToList();
            }

            fullLoadingTypes.Add(type);
            try
            {
                var items = (await GetAllItemsAsync(type, included)).OfType<ICacheable>();
                fullLoadedTypes.Add(type);

                if (Data.ContainsKey(type)) // remove deleted entities from cache
                {
                    var toRemove = Data[type].Where(p => items.All(i => i.Id != p.Value.Id)).ToList();
                    foreach (var r in toRemove)
                        Data[type].Remove(r.Key);
                }
                return AddItems(items);
            }
            catch (Exception ex)
            {
                CompleteWaitingWithException(type, 0, null, ex);
            }
            finally
            {
                CompleteWaiting(type, 0, null);
                fullLoadingTypes.Remove(type);
            }
            return null;
        }

        public Task LoadPropertyForItemAsync<T, TProperty>(T item, Expression<Func<T, object>> propertyName, bool reload = true, params string[] included)
            where T : ICacheable
            where TProperty : ICacheable
        {
            return LoadPropertyForItemAsync<T, TProperty>(item, ExpressionHelper.GetName(propertyName), reload, included);
        }

        //public Task LoadPropertyForItemAsync<T, TProperty>(T item, Expression<Func<T, object>> propertyName, bool reload = true, params Expression<Func<T, object>>[] included)
        //	where T : ICacheable
        //	where TProperty : ICacheable
        //{
        //	var list = new List<string>();
        //	foreach (var inc in included)
        //		list.Add(ExpressionHelper.GetName(inc));
        //	return LoadPropertyForItemAsync<T, TProperty>(item, ExpressionHelper.GetName(propertyName), reload, list.ToArray());
        //}

        public async Task LoadPropertyForItemAsync<T, TProperty>(T item, string propertyName, bool reload = true, params string[] included)
            where T : ICacheable
            where TProperty : ICacheable
        {
            var type = item.GetType();
            var baseType = GetBaseType(typeof(T));
            var itemType = baseType.Name;

            var property = ReflectionUtilities.GetProperty(type, propertyName); 
            var isCollectionProperty = typeof(IList).GetTypeInfo().IsAssignableFrom(property.PropertyType.GetTypeInfo());

            // check loaded
            int? foreignId = 0; 
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
                    var idProperty = ReflectionUtilities.GetProperty(type, propertyName + "Id");
                    if (idProperty.PropertyType == typeof(int?))
                    {
                        foreignId = (int?)idProperty.GetValue(item, null);

                        // sync navigation proprety with foreign id
                        if (!foreignId.HasValue) 
                        {
                            if (currentValue != null)
                                property.SetValue(item, null, null);
                            return; 
                        }
                        else
                        {
                            if (currentValue != null && ((ICacheable) currentValue).Id == foreignId.Value)
                                return;
                            property.SetValue(item, null, null);
                        }
                    }
                    else
                    {
                        var id = (int)idProperty.GetValue(item, null);
                        if (currentValue != null && ((ICacheable)currentValue).Id == id)
                            return;
                    }
                }
            }

            // check IsLoading (already loading)
            var loadingProperty = ReflectionUtilities.GetProperty(type, "Is" + propertyName + "Loading"); 
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
                Debug.WriteLine("DataManagerBase - Warning: No 'Is" + propertyName + "Loading' property found on '" + type.Name + "'");

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
                    var result = await GetItemAsync(GetBaseType(property.PropertyType).Name, foreignId.Value, included ?? new string[] { });
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

        private readonly List<Tuple<string, int, string, TaskCompletionSource<object>>> waiting =
            new List<Tuple<string, int, string, TaskCompletionSource<object>>>();

        private Task WaitUntilCompleted(string type, int id, string propertyName)
        {
            var task = new TaskCompletionSource<object>();
            waiting.Add(new Tuple<string, int, string, TaskCompletionSource<object>>(type, id, propertyName, task));
            return task.Task;
        }

        private void CompleteWaiting(string type, int id, string propertyName)
        {
            var finished = waiting.Where(t => t.Item1 == type && t.Item2 == id && t.Item3 == propertyName).ToArray();
            foreach (var f in finished)
            {
                f.Item4.SetResult(null);
                waiting.Remove(f);
            }
        }

        private void CompleteWaitingWithException(string type, int id, string propertyName, Exception ex)
        {
            var finished = waiting.Where(t => t.Item1 == type && t.Item2 == id && t.Item3 == propertyName).ToArray();
            foreach (var f in finished)
            {
                f.Item4.SetException(ex);
                waiting.Remove(f);
            }
        }
    }

#if WP7
    internal class HashSet<T> : List<T> 
    {
        
    }
#endif
}