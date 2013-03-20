using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace MyToolkit.Caching
{
	public interface ICacheable
	{
		int Id { get; }
	}

	public class CacheManager
	{
		public Dictionary<Type, Dictionary<int, ICacheable>> list = new Dictionary<Type, Dictionary<int, ICacheable>>();

		public T GetItem<T>(int id) where T : ICacheable
		{
			return (T)GetItem(typeof(T), id);
		}

		public T GetItem<T>(T item) where T : ICacheable
		{
			return (T)GetItem(item.GetType(), item.Id);
		}

		private void AddItem(ICacheable item)
		{
			var type = GetBaseType(item.GetType());
			if (!list.ContainsKey(type))
			{
				var group = new Dictionary<int, ICacheable>();
				list[type] = @group;
			}
			list[type][item.Id] = item;
		}

		public ICacheable GetItem(Type type, int id)
		{
			type = GetBaseType(type);
			if (list.ContainsKey(type))
			{
				var group = list[type];
				if (group.ContainsKey(id))
					return group[id];
			}
			return null; 
		}
		
		public T SetItem<T>(T item) where T : ICacheable
		{
			return (T)SetItem(item, new HashSet<ICacheable>(), true);
		}

		private ICacheable SetItem(ICacheable item, HashSet<ICacheable> mergedObjects, bool mergeFields)
		{
			var isMerging = false; 
			var currentItem = GetItem(item.GetType(), item.Id);			
			if (currentItem == null)
			{
				currentItem = item;
				AddItem(item);
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
				foreach (var property in type.GetProperties())
				{
					var attr = property.GetCustomAttributes(typeof(DataMemberAttribute), true).FirstOrDefault();
					if (attr != null)
					{
						var isList = false;
						var isCacheableProperty = false; 

						var value = property.GetValue(item, null);
						if (value != null)
						{
							isList = value.GetType().Name.StartsWith("ObservableCollection");
							
							if (value is ICacheable)
							{
								value = SetItem((ICacheable)value, mergedObjects, !mergedObjects.Contains((ICacheable)value));
								isCacheableProperty = true; 
							}

							if (isList)
							{
								var listType = value.GetType();
								var listItemType = listType.GetGenericArguments()[0];

								if (listItemType.GetInterfaces().Contains(typeof(ICacheable)))
								{
									var ofType = typeof(Enumerable).GetMethod("OfType");
									ofType = ofType.MakeGenericMethod(listItemType);

									isCacheableProperty = true;
									value = Activator.CreateInstance(listType, ofType.Invoke(null,
									    new object[] {
										    ((IEnumerable<ICacheable>)value)
										    .Select(i => SetItem(i, mergedObjects, !mergedObjects.Contains(i)))
										    .OfType<object>()
										    .ToArray()
									    })
									);
								}
							}
						}

						if ((isMerging || isCacheableProperty) && property.CanWrite)
						{
							if (value == null)
							{
								var idProperty = type.GetProperty(property.Name + "Id");
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

		private Type GetBaseType(Type type)
		{
			if (type.BaseType.Name == "EntityObject")
				return type;
			return GetBaseType(type.BaseType);
		}

		public IList<T> SetItems<T>(IEnumerable<T> items) where T : class
		{
			var list = new List<T>();
			var mergedObjects = new HashSet<ICacheable>();
			foreach (var item in items)
			{
				if (item is ICacheable)
					list.Add((T)SetItem((ICacheable)item, mergedObjects, true));
				else
					list.Add(item);
			}
			return list;
		}
	}
}