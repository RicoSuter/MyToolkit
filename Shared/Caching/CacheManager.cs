using System;
using System.Collections;
using System.Collections.Generic;
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
		public HashSet<ICacheable> list = new HashSet<ICacheable>();

		public T GetItem<T>(int id) where T : ICacheable
		{
			return list.OfType<T>().SingleOrDefault(i => i.Id == id);
		}

		public ICacheable GetItem(Type type, int id)
		{
			return list.SingleOrDefault(i => i.GetType() == type && i.Id == id);
		}

		public T GetItem<T>(T item) where T : ICacheable
		{
			var currentItem = GetItem(typeof(T), item.Id);
			if (currentItem == null)
				return item;
			return (T)currentItem;
		}

		public T SetItem<T>(T item) where T : ICacheable
		{
			return (T)SetItem(item, new List<ICacheable>(), true);
		}

		private ICacheable SetItem(ICacheable item, List<ICacheable> mergedObjects, bool mergeFields)
		{
			mergedObjects.Add(item); // used to avoid recursions

			var currentItem = GetItem(item.GetType(), item.Id);
			if (currentItem == null)
			{
				list.Add(item);
				return item;
			}

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
						var value = property.GetValue(item, null);
						if (value != null)
						{
							isList = value.GetType().Name.StartsWith("ObservableCollection");

							if (value is ICacheable)
								value = SetItem((ICacheable)value, mergedObjects, !mergedObjects.Contains((ICacheable)value));

							if (isList)
							{
								var listType = value.GetType();
								var listItemType = listType.GetGenericArguments()[0];

								if (listItemType.GetInterfaces().Contains(typeof(ICacheable)))
								{
									var ofType = typeof(Enumerable).GetMethod("OfType");
									ofType = ofType.MakeGenericMethod(listItemType);

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
			return currentItem;
		}

		public IList<T> SetItems<T>(IEnumerable<T> items) where T : class
		{
			var list = new List<T>();
			foreach (var item in items)
			{
				if (item is ICacheable)
					list.Add((T)SetItem((ICacheable)item));
				else
					list.Add(item);
			}
			return list;
		}
	}
}