//-----------------------------------------------------------------------
// <copyright file="ApplicationSettings.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyToolkit.Storage
{
#if !LEGACY

    /// <summary>Provides methods to access and write settings to the isolated storage (works only for WP and WinRT). </summary>
    public static class ApplicationSettings
    {
        private static readonly IApplicationSettings _applicationSettings;

        static ApplicationSettings()
        {
            if (ApplicationSettingsWp8.IsActive)
                _applicationSettings = new ApplicationSettingsWp8();
            else if (ApplicationSettingsWpf.IsActive)
                _applicationSettings = new ApplicationSettingsWpf();
            else if (ApplicationSettingsWinRt.IsActive)
                _applicationSettings = new ApplicationSettingsWinRt();
        }

        /// <summary>Gets a value indicating whether the settings can be roamed (synced across various devices). </summary>
        public static bool CanRoam
        {
            get { return _applicationSettings.CanRoam; }
        }

        /// <summary>Sets a setting in the isolated storage. </summary>
        /// <typeparam name="T">The type of the setting. </typeparam>
        /// <param name="key">The key of the setting. </param>
        /// <param name="value">The value of the setting. </param>
        /// <param name="roaming">True if the setting should be roamed to other devices. </param>
        /// <param name="save">True if the the change should be written to the isolated storage. </param>
        public static void SetSetting<T>(string key, T value, bool roaming = false, bool save = false)
        {
            if (_applicationSettings == null)
                throw new NotImplementedException();

            _applicationSettings.SetSetting(key, value, roaming, save);
        }

        /// <summary>Gets a setting from the isolated storage. </summary>
        /// <typeparam name="T">The type of the setting. </typeparam>
        /// <param name="key">The key of the setting. </param>
        /// <returns>The setting. </returns>
        public static T GetSetting<T>(string key)
        {
            return GetSetting(key, default(T));
        }

        /// <summary>Gets a setting from the isolated storage. </summary>
        /// <typeparam name="T">The type of the setting. </typeparam>
        /// <param name="key">The key of the setting. </param>
        /// <param name="defaultValue">The default value of the settings (returned if it is not currently set). </param>
        /// <param name="roaming">True if the setting is roamed to other devices. </param>
        /// <returns>The setting. </returns>
        public static T GetSetting<T>(string key, T defaultValue, bool roaming = false)
        {
            if (_applicationSettings == null)
                throw new NotImplementedException();

            return _applicationSettings.GetSetting(key, defaultValue, roaming);
        }

        /// <summary>Check whether a setting exists in the isolated storage. </summary>
        /// <typeparam name="T">The type of the setting. </typeparam>
        /// <param name="key">The key of the setting. </param>
        /// <param name="roaming">True if the setting is roamed to other devices. </param>
        /// <returns>True if setting exists. </returns>
        public static bool HasSetting<T>(string key, bool roaming = false)
        {
            if (_applicationSettings == null)
                throw new NotImplementedException();

            return _applicationSettings.HasSetting<T>(key, roaming);
        }

        /// <summary>Removes a setting from the isolated storage. </summary>
        /// <param name="key">The key of the setting. </param>
        /// <param name="roaming">True if the setting is roamed to other devices. </param>
        /// <param name="save">True if the removal should be written to the isolated storage. </param>
        /// <returns>Returns true if the setting has successfully removed. If setting is not present, false is returned. </returns>
        public static bool RemoveSetting(string key, bool roaming = false, bool save = false)
        {
            if (_applicationSettings == null)
                throw new NotImplementedException();

            return _applicationSettings.RemoveSetting(key, roaming, save);
        }
    }

#else

    /// <summary>Provides methods to access and write settings to the isolated storage (works only for WP and WinRT). </summary>
    public static class ApplicationSettings
    {
        /// <summary>Sets a setting in the isolated storage. </summary>
        /// <typeparam name="T">The type of the setting. </typeparam>
        /// <param name="key">The key of the setting. </param>
        /// <param name="value">The value of the setting. </param>
        /// <param name="roaming">True if the setting should be roamed to other devices. </param>
        /// <param name="save">True if the the change should be written to the isolated storage. </param>
        public static void SetSetting<T>(string key, T value, bool roaming = false, bool save = false)
        {
            var type = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows");
            if (type == null)
                type = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows, Version=2.0.6.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");

            if (type != null)
            {
                var settings = type.GetProperty("ApplicationSettings").GetValue(null, null);
                var settingsType = settings.GetType();
                settingsType.GetProperty("Item").SetValue(settings, value, new object[] {key});
                if (save)
                    settingsType.GetMethod("Save").Invoke(settings, null);
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>Gets a setting from the isolated storage. </summary>
        /// <typeparam name="T">The type of the setting. </typeparam>
        /// <param name="key">The key of the setting. </param>
        /// <returns>The setting. </returns>
        public static T GetSetting<T>(string key)
        {
            return GetSetting(key, default(T));
        }

        /// <summary>Gets a setting from the isolated storage. </summary>
        /// <typeparam name="T">The type of the setting. </typeparam>
        /// <param name="key">The key of the setting. </param>
        /// <param name="defaultValue">The default value of the settings (returned if it is not currently set). </param>
        /// <param name="roaming">True if the setting is roamed to other devices. </param>
        /// <returns>The setting. </returns>
        public static T GetSetting<T>(string key, T defaultValue, bool roaming = false)
        {
            var type = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows");
            if (type == null)
                type = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows, Version=2.0.6.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");

            if (type != null)
            {
                var settings = type.GetProperty("ApplicationSettings").GetValue(null, null);
                var settingsType = settings.GetType();
                if ((bool) settingsType.GetMethod("Contains").Invoke(settings, new object[] {key}))
                {
                    var value = settingsType.GetProperty("Item").GetValue(settings, new object[] {key});
                    if (value is T)
                        return (T) value;
                    return defaultValue;
                }
                return defaultValue;
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>Check whether a setting exists in the isolated storage. </summary>
        /// <typeparam name="T">The type of the setting. </typeparam>
        /// <param name="key">The key of the setting. </param>
        /// <param name="roaming">True if the setting is roamed to other devices. </param>
        /// <returns>True if setting exists. </returns>
        public static bool HasSetting<T>(string key, bool roaming = false)
        {
            var type = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows");
            if (type == null)
                type = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows, Version=2.0.6.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");

            if (type != null)
            {
                var settings = type.GetProperty("ApplicationSettings").GetValue(null, null);
                var settingsType = settings.GetType();
                if ((bool) settingsType.GetMethod("Contains").Invoke(settings, new object[] {key}))
                {
                    var value = settingsType.GetProperty("Item").GetValue(settings, new object[] {key});
                    if (value is T)
                        return true;
                    return false;
                }
                return false;
            }
            else
                throw new NotImplementedException();
        }

        /// <summary>Removes a setting from the isolated storage. </summary>
        /// <param name="key">The key of the setting. </param>
        /// <param name="roaming">True if the setting is roamed to other devices. </param>
        /// <param name="save">True if the removal should be written to the isolated storage. </param>
        /// <returns>Returns true if the setting has successfully removed. If setting is not present, false is returned. </returns>
        public static bool RemoveSetting(string key, bool roaming = false, bool save = true)
        {
            var type = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows");
            if (type == null)
                type = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows, Version=2.0.6.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");

            if (type != null)
            {
                var settings = type.GetProperty("ApplicationSettings").GetValue(null, null);
                var settingsType = settings.GetType();
                if ((bool) settingsType.GetMethod("Contains").Invoke(settings, new object[] {key}))
                {
                    var value = (bool) settingsType.GetMethod("Remove").Invoke(settings, new object[] {key});
                    if (value && save)
                        settingsType.GetMethod("Save").Invoke(settings, null);
                    return save;
                }
                return false;
            }
            else
                throw new NotImplementedException();
        }
    }

#endif
}
