using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Messaging;
#if !SL4
using MyToolkit.Serialization;
using MyToolkit.Storage;
#endif

namespace MyToolkit.Utilities
{
	public static class EntityContainerExtensions
	{
#if WP7 || WP8

        public static void SaveToFile<TEntity, TIdentity>(this EntityContainer<TEntity, TIdentity> container, string fileName)
            where TEntity : class, IEntity<TIdentity>
		{
            FileUtilities.WriteAllText(fileName, DataContractSerialization.Serialize<List<TEntity>>(container.Collection.ToList()));
		}

        public static void LoadFromFile<TEntity, TIdentity>(this EntityContainer<TEntity, TIdentity> container, string fileName, List<TEntity> defaultList = null)
            where TEntity : class, IEntity<TIdentity>
		{
			var data = FileUtilities.ReadAllText(fileName);
			if (data != null)
				container.Collection.Initialize(DataContractSerialization.Deserialize<List<TEntity>>(data));
			else if (defaultList != null)
				container.Collection.Initialize(defaultList);
			else
				container.Collection.Clear();
		}

#endif

#if !WP7 && !SL4 && !SL5 && !WPF

        public static async Task SaveToFileAsync<TEntity, TIdentity>(this EntityContainer<TEntity, TIdentity> me, string fileName) 
            where TEntity : class, IEntity<TIdentity>
		{
			await FileUtilities.WriteAllTextAsync(fileName, await DataContractSerialization.SerializeAsync(me.Collection.ToList()));
		}

        public static async Task LoadFromFileAsync<T, TIdentity>(this EntityContainer<T, TIdentity> me, string fileName, List<T> defaultList = null) 
            where T : class, IEntity<TIdentity>
		{
			var data = await FileUtilities.ReadAllTextAsync(fileName);
			if (data != null)
				me.Collection.Initialize(await DataContractSerialization.DeserializeAsync<List<T>>(data));
			else if (defaultList != null)
				me.Collection.Initialize(defaultList);
			else
				me.Collection.Clear();
		}

#endif
	}
}
