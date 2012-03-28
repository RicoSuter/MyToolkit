using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MyToolkit.UI
{
	public static class Thombstone
	{
		public static bool HasBeenActivated
		{
			get { return PhoneApplicationService.Current.StartupMode == StartupMode.Activate;  }
		}

		public static void SaveState(this PhoneApplicationPage page, string key, object value)
		{
			if (page.State.ContainsKey(key))
				page.State.Remove(key);
			page.State[key] = value; 
		}

		public static T ReadState<T>(this PhoneApplicationPage page, string key)
		{
			return ReadState(page, key, default(T));
		}

		public static T ReadState<T>(this PhoneApplicationPage page, string key, T defaultValue)
		{
			if (page.State.ContainsKey(key))
				return (T) page.State[key];
			return defaultValue; 
		}

		public static void SaveGlobalState(string key, object value)
		{
			if (PhoneApplicationService.Current.State.ContainsKey(key))
				PhoneApplicationService.Current.State.Remove(key);
			PhoneApplicationService.Current.State[key] = value;
		}

		public static T ReadGlobalState<T>(string key)
		{
			if (PhoneApplicationService.Current.State.ContainsKey(key))
				return (T)PhoneApplicationService.Current.State[key];
			return default(T);
		}
	}
}
