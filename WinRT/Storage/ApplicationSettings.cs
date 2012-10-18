using System;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MyToolkit.Storage
{
	public static class ApplicationSettings
	{
		public static void SetSetting<T>(string key, T value, bool roaming = false)
		{
			var settings = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
			settings.Values[key] = value;
		}

		public static T GetSetting<T>(string key, bool roaming = true)
		{
			return GetSetting(key, default(T), roaming);
		}

		public static T GetSetting<T>(string key, T defaultValue, bool roaming = false)
		{
			var settings = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
			return settings.Values.ContainsKey(key) &&
				   settings.Values[key] is T ?
				   (T)settings.Values[key] : defaultValue;
		}

		public static bool HasSetting<T>(string key, bool roaming = false)
		{
			var settings = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
			return settings.Values.ContainsKey(key) && settings.Values[key] is T;
		}

		public static bool RemoveSetting(string key, bool roaming = false)
		{
			var settings = roaming ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
			if (settings.Values.ContainsKey(key))
				return settings.Values.Remove(key);
			return false;
		}

		public static async Task SetSettingToFileAsync<T>(string key, T value, bool roaming = false)
		{
			var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting) :
				await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting);

			await FileIO.WriteTextAsync(file, Xml.Serialize(value), UnicodeEncoding.Utf8);
		}

		public static async Task<T> GetSettingFromFileAsync<T>(string key, T defaultValue, bool roaming = false)
		{
			var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists) :
				await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists);

			var xml = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
			return !String.IsNullOrEmpty(xml) ? Xml.Deserialize<T>(xml) : defaultValue;
		}
	}
}
