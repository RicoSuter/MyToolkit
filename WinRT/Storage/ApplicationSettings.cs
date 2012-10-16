using System;
using MyToolkit.Utilities;
using Windows.Storage;

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

		public static void SetXmlSetting<T>(string key, T value, bool roaming = false)
		{
			SetSetting(key, Xml.Serialize(value), roaming);
		}

		public static T GetXmlSetting<T>(string key, T defaultValue, bool roaming = false)
		{
			var xml = GetSetting<string>(key, null, roaming);
			if (xml != null)
				return Xml.Deserialize<T>(xml);
			return defaultValue;
		}
	}
}
