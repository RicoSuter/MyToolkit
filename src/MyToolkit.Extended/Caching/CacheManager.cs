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

namespace MyToolkit.Caching
{
    public interface ICacheable
    {
        int Id { get; }
    }

    public class CacheManager
    {
        public CacheManager()
        {
            Data = new Dictionary<string, Dictionary<int, ICacheable>>();
        }

        protected Dictionary<string, Dictionary<int, ICacheable>> Data { get; private set; }

        protected T GetItem<T>(int id) where T : ICacheable
        {
            return (T)GetItem(typeof(T), id);
        }

        protected ICacheable GetItem(Type type, int id)
        {
            return GetItem(GetBaseType(type).Name, id);
        }

        protected ICacheable GetItem(string type, int id)
        {
            if (Data.ContainsKey(type))
            {
                var group = Data[type];
                if (group.ContainsKey(id))
                    return group[id];
            }
            return null;
        }

        public IList<T> AddItems<T>(IEnumerable<T> items) where T : class
        {
            var list = new List<T>();
            var mergedObjects = new HashSet<ICacheable>();
            foreach (var item in items)
            {
                if (item is ICacheable)
                    list.Add((T)AddItem((ICacheable)item, mergedObjects, true));
                else
                    list.Add(item);
            }
            return list;
        }

        protected T AddItem<T>(T item) where T : ICacheable
        {
            return (T)AddItem(item, new HashSet<ICacheable>(), true);
        }

        protected ICacheable AddItem(ICacheable item)
        {
            return AddItem(item, new HashSet<ICacheable>(), true);
        }

        private ICacheable AddItem(ICacheable item, HashSet<ICacheable> mergedObjects, bool mergeFields)
        {
            var isMerging = false; 
            var currentItem = GetItem(item.GetType(), item.Id);			
            if (currentItem == null)
            {
                currentItem = item;

                // add item to list
                var type = GetBaseType(item.GetType());
                if (!Data.ContainsKey(type.Name))
                {
                    var group = new Dictionary<int, ICacheable>();
                    Data[type.Name] = @group;
                }
                Data[type.Name][item.Id] = item;
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
                foreach (var property in type.GetRuntimeProperties())
                {
                    var attr = property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault();
                    if (attr != null)
                    {
                        var isCacheableProperty = false; 

                        var value = property.GetValue(item, null);
                        if (value != null)
                        {
                            if (value is ICacheable)
                            {
                                value = AddItem((ICacheable)value, mergedObjects, !mergedObjects.Contains((ICacheable)value));
                                isCacheableProperty = true; 
                            }
                            else if (value is IList)
                            {
                                var listType = value.GetType();
                                var genericArguments = ReflectionUtilities.GetGenericArguments(listType);
                                if (genericArguments.Count == 1)
                                {
                                    var listItemType = genericArguments[0];
                                    if (ReflectionUtilities.GetInterfaces(listItemType).Contains(typeof(ICacheable)))
                                    {
                                        var ofType = typeof(Enumerable).GetRuntimeMethod("OfType", null);
                                        ofType = ofType.MakeGenericMethod(listItemType);

                                        isCacheableProperty = true;
                                        value = Activator.CreateInstance(listType, ofType.Invoke(null,
                                            new object[] {
                                                ((IEnumerable<ICacheable>)value)
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
                                var idProperty = type.GetRuntimeProperty(property.Name + "Id");
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

        protected virtual Type GetBaseType(Type type)
        {
#if WINRT
            if (ReflectionUtilities.GetBaseType(type).Name == "EntityObject")
                return type;

            return GetBaseType(ReflectionUtilities.GetBaseType(type));
#else
            if (type.BaseType.Name == "EntityObject")
                return type;

            return GetBaseType(type.BaseType);
#endif
        }
    }
}