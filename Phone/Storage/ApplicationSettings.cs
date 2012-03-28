using System.IO.IsolatedStorage;

namespace MyToolkit.Storage
{
	public static class ApplicationSettings
	{
		public static void SetSetting<T>(string key, T value, bool save = false)
		{
			IsolatedStorageSettings.ApplicationSettings[key] = value; 
			if (save)
				IsolatedStorageSettings.ApplicationSettings.Save();
		}

		public static T GetSetting<T>(string key)
		{
			return GetSetting(key, default(T));
		}

		public static T GetSetting<T>(string key, T defaultValue)
		{
			return IsolatedStorageSettings.ApplicationSettings.Contains(key) && 
					IsolatedStorageSettings.ApplicationSettings[key] is T
				? (T) IsolatedStorageSettings.ApplicationSettings[key]
				: defaultValue; 
		}

		public static bool HasSetting<T>(string key)
		{
			return IsolatedStorageSettings.ApplicationSettings.Contains(key) && 
				IsolatedStorageSettings.ApplicationSettings[key] is T;
		}
	}
}
