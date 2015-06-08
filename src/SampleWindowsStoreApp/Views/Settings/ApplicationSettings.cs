namespace SampleWindowsStoreApp.Views.Settings
{
    // TODO: Move this class to appropriate location. 
    public class ApplicationSettings
    {
        // Default way to read/write settings in WinRT
        public static int IntSetting
        {
            get
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (settings.Values.ContainsKey("IntSetting"))
                    return (int)settings.Values["IntSetting"];
                return 10; // default value
            }
            set
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                settings.Values["IntSetting"] = value;
            }
        }

        // Read/write setting using MyToolkit library (ApplicationSettings class in PCL to use in view models)
        public static string StringSetting
        {
            get { return MyToolkit.Storage.ApplicationSettings.GetSetting("StringSetting", "DefaultValue"); }
            set { MyToolkit.Storage.ApplicationSettings.SetSetting("StringSetting", value); }
        }

        // Read/write roaming setting (synced across devices)
        public static bool BooleanSetting
        {
            get { return MyToolkit.Storage.ApplicationSettings.GetSetting("BooleanSetting", true, true); }
            set { MyToolkit.Storage.ApplicationSettings.SetSetting("BooleanSetting", value, true); }
        }
    }
}
