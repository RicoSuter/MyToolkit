using System;
using System.Threading.Tasks;
using MyToolkit.Serialization;
using MyToolkit.Utilities;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MyToolkit.Storage
{
	public static class ApplicationSettingsEx
	{
		public static async Task SetSettingToFileAsync<T>(string key, T value, bool roaming = false, Type[] extraTypes = null)
		{
			var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting) :
				await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting);

			var xml = DataContractSerialization.Serialize(value, true, extraTypes); 
			await FileIO.WriteTextAsync(file, xml, UnicodeEncoding.Utf8);
		}

		public static async Task<T> GetSettingFromFileAsync<T>(string key, T defaultValue, bool roaming = false, Type[] extraTypes = null)
		{
			var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists) :
				await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists);

			var xml = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
			return !String.IsNullOrEmpty(xml) ? DataContractSerialization.Deserialize<T>(xml, extraTypes) : defaultValue;
		}

		public static async Task SetSettingToXmlFileAsync<T>(string key, T value, bool roaming = false, Type[] extraTypes = null)
		{
			var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting) :
				await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting);

			var xml = XmlSerialization.Serialize(value, extraTypes);
			await FileIO.WriteTextAsync(file, xml, UnicodeEncoding.Utf8);
		}

		public static async Task<T> GetSettingFromXmlFileAsync<T>(string key, T defaultValue, bool roaming = false, Type[] extraTypes = null)
		{
			var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists) :
				await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists);

			var xml = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
			return !String.IsNullOrEmpty(xml) ? XmlSerialization.Deserialize<T>(xml, extraTypes) : defaultValue;
		}
	}
}
