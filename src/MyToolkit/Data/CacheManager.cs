//-----------------------------------------------------------------------
// <copyright file="CacheManager.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using MyToolkit.Utilities;

namespace MyToolkit.Data
{
    internal class HashSet<T> : List<T> { }

    /// <summary>Provides a cache per object type and automatically merges updated objects into old objects. </summary>
    public class CacheManager<TIdentity>
    {
        /// <summary>Gets or sets the internal list of managed items. </summary>
        protected Dictionary<string, Dictionary<TIdentity, IEntity<TIdentity>>> List
            = new Dictionary<string, Dictionary<TIdentity, IEntity<TIdentity>>>();

        /// <summary>Returns an item by ID. </summary>
        /// <typeparam name="T">Type of the item. </typeparam>
        /// <param name="id">ID of the item. </param>
        /// <returns>The entity.</returns>
        protected T GetItem<T>(TIdentity id) where T : IEntity<TIdentity>
        {
            return (T)GetItem(typeof(T), id);
        }

        /// <summary>Returns an item by ID. </summary>
        /// <param name="type">Type of the item. </param>
        /// <param name="id">ID of the item. </param>
        /// <returns>The entity.</returns>
        protected IEntity<TIdentity> GetItem(Type type, TIdentity id)
        {
            return GetItem(GetBaseType(type).Name, id);
        }

        /// <summary>Returns an item by ID. </summary>
        /// <param name="type">Type of the item (only class name). </param>
        /// <param name="id">ID of the item. </param>
        /// <returns>The entity.</returns>
        protected IEntity<TIdentity> GetItem(string type, TIdentity id)
        {
            if (List.ContainsKey(type))
            {
                var group = List[type];
                if (group.ContainsKey(id))
                    return group[id];
            }
            return null;
        }

        /// <summary>Adds mulitple items to the cache manager. </summary>
        /// <typeparam name="T">Type of the items. </typeparam>
        /// <param name="items">The items to add. </param>
        /// <returns>The list with the added itmes. </returns>
        public IList<T> AddItems<T>(IEnumerable<T> items) where T : class
        {
            var list = new List<T>();
            var mergedObjects = new HashSet<IEntity<TIdentity>>();
            foreach (var item in items)
            {
                if (item is IEntity<TIdentity>)
                    list.Add((T)AddItem((IEntity<TIdentity>)item, mergedObjects, true));
                else
                    list.Add(item);
            }
            return list;
        }

        /// <summary>Adds an item to the cache manager and returns the used item, 
        /// either the item itself or the already existing item with the updated properties. </summary>
        /// <param name="item">The new item. </param>
        /// <returns>The used item. </returns>
        protected T AddItem<T>(T item) where T : IEntity<TIdentity>
        {
            return (T)AddItem(item, new HashSet<IEntity<TIdentity>>(), true);
        }

        /// <summary>Adds an item to the cache manager and returns the used item, 
        /// either the item itself or the already existing item with the updated properties. </summary>
        /// <param name="item">The new item. </param>
        /// <returns>The used item. </returns>
        protected IEntity<TIdentity> AddItem(IEntity<TIdentity> item)
        {
            return AddItem(item, new HashSet<IEntity<TIdentity>>(), true);
        }

        private IEntity<TIdentity> AddItem(IEntity<TIdentity> item, HashSet<IEntity<TIdentity>> mergedObjects, bool mergeFields)
        {
            var isMerging = false;
            var currentItem = GetItem(item.GetType(), item.Id);
            if (currentItem == null)
            {
                currentItem = item;

                // add item to list
                var type = GetBaseType(item.GetType());
                if (!List.ContainsKey(type.Name))
                {
                    var group = new Dictionary<TIdentity, IEntity<TIdentity>>();
                    List[type.Name] = @group;
                }
                List[type.Name][item.Id] = item;
            }
            else
                isMerging = true;

            if (mergedObjects.Contains(currentItem))
                return currentItem;

            mergedObjects.Add(currentItem); // used to avoid recursions

            if (isMerging)
                Debug.WriteLine("CacheManager: Merging item " + item.GetType().Name + "." + item.Id);
            else
                Debug.WriteLine("CacheManager: Adding item " + item.GetType().Name + "." + item.Id);

            // copy new values into old object
            if (mergeFields)
            {
                var type = item.GetType();
#if !LEGACY
                foreach (var property in type.GetRuntimeProperties())
#else
                foreach (var property in type.GetProperties())
#endif
                {
                    var attr = property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault();
                    if (attr != null)
                    {
                        var isCacheableProperty = false;

                        var value = property.GetValue(item, null);
                        if (value != null)
                        {
                            if (value is IEntity<TIdentity>)
                            {
                                value = AddItem((IEntity<TIdentity>)value, mergedObjects, !mergedObjects.Contains((IEntity<TIdentity>)value));
                                isCacheableProperty = true;
                            }
                            else if (value is IList)
                            {
                                var listType = value.GetType();
#if !LEGACY
                                var genericArguments = listType.GenericTypeArguments;
#else
                                var genericArguments = listType.GetGenericArguments();
#endif
                                if (genericArguments.Count() == 1)
                                {
                                    var listItemType = genericArguments[0];
#if !LEGACY
                                    if (listItemType.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEntity<TIdentity>)))
                                    {
                                        var ofType = typeof(Enumerable).GetRuntimeMethod("OfType", new Type[] { typeof(IEnumerable) });
                                        ofType = ofType.MakeGenericMethod(listItemType);
#else
                                    if (listItemType.GetInterfaces().Contains(typeof(IEntity<TIdentity>)))
                                    {
                                        var ofType = typeof(Enumerable).GetMethod("OfType", new Type[] { typeof(IEnumerable) });
                                        ofType = ofType.MakeGenericMethod(listItemType);
#endif

                                        isCacheableProperty = true;
                                        value = Activator.CreateInstance(listType, ofType.Invoke(null,
                                            new object[] {
                                                ((IEnumerable<IEntity<TIdentity>>)value)
                                                .Select(i => AddItem(i, mergedObjects, !mergedObjects.Contains(i)))
                                                .OfType<object>()
                                                .ToArray()
                                            })
                                        );
                                    }
                                }
                            }
                        }

                        if ((isMerging || isCacheableProperty) && property.CanWrite)
                        {
                            if (value == null)
                            {
#if !LEGACY
                                var idProperty = type.GetRuntimeProperty(property.Name + "Id");
#else
                                var idProperty = type.GetProperty(property.Name + "Id");
#endif
                                if (idProperty != null) // check if value is null or not loaded
                                {
                                    var id = (int?)idProperty.GetValue(item, null);
                                    if (!id.HasValue) // if property changed to null => set null in current cached object
                                        property.SetValue(currentItem, null, null);
                                }
                            }
                            else
                                property.SetValue(currentItem, value, null);
                        }
                    }
                }
            }

            return currentItem;
        }

        /// <summary>Gets the base type of the given type. </summary>
        /// <param name="type">The type to find the base type for. </param>
        /// <returns>The base type. </returns>
        protected virtual Type GetBaseType(Type type)
        {
#if WINRT
            if (type.GetBaseType().Name == "EntityObject")
                return type;
            return GetBaseType(type.GetBaseType());
#elif LEGACY
            if (type.BaseType.Name == "EntityObject")
                return type;
            return GetBaseType(type.BaseType);
#else
            if (type.GetTypeInfo().BaseType.Name == "EntityObject")
                return type;
            return GetBaseType(type.GetTypeInfo().BaseType);
#endif
        }
    }
}