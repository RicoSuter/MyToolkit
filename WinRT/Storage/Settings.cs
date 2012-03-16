using System;
using Windows.Storage;

namespace MyToolkit.Storage
{
	public static class Settings
	{
		public static void SetSetting<T>(string key, T value)
		{
			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values[key] = value;
		}

		public static T GetSetting<T>(string key)
		{
			return GetSetting(key, default(T));
		}

		public static T GetSetting<T>(string key, T defaultValue)
		{
			var localSettings = ApplicationData.Current.LocalSettings;
			return localSettings.Values.ContainsKey(key) &&
				   localSettings.Values[key] is T ?
				   (T)localSettings.Values[key] : defaultValue;
		}

		public static bool HasSetting<T>(string key)
		{
			var localSettings = ApplicationData.Current.LocalSettings;
			return localSettings.Values.ContainsKey(key) && localSettings.Values[key] is T;
		}
	}
}
