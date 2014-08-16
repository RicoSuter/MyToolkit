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
        private static IDictionary<string, object> GetWinRtSettings(Type type, bool roaming)
        {
            var data = type.GetRuntimeProperty("Current").GetValue(null, null);
            var settings = roaming
                ? data.GetType().GetRuntimeProperty("RoamingSettings").GetValue(data, null)
                : data.GetType().GetRuntimeProperty("LocalSettings").GetValue(data, null);
            return (IDictionary<string, object>)settings.GetType().GetRuntimeProperty("Values").GetValue(settings, null);
        }

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
                var settings = type.GetRuntimeProperty("ApplicationSettings").GetValue(null, null);
                var settingsType = settings.GetType();
                settingsType.GetRuntimeProperty("Item").SetValue(settings, value, new object[] { key });
                if (save)
                    settingsType.GetRuntimeMethod("Save", new Type[] { }).Invoke(settings, null);
            }
            else
            {
                type = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime");
                if (type != null)
                {
                    var settings = GetWinRtSettings(type, roaming);
                    settings[key] = value;
                }
                else
                    throw new NotImplementedException();
            }
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
                var settings = type.GetRuntimeProperty("ApplicationSettings").GetValue(null, null);
                var settingsType = settings.GetType();
                if ((bool)settingsType.GetRuntimeMethod("Contains", new Type[] { typeof(string) }).Invoke(settings, new object[] { key }))
                {
                    var value = settingsType.GetRuntimeProperty("Item").GetValue(settings, new object[] { key });
                    if (value is T)
                        return (T)value;
                    return defaultValue;
                }
                return defaultValue;
            }
            else
            {
                type = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime");
                if (type != null)
                {
                    var settings = GetWinRtSettings(type, roaming);
                    if (settings.ContainsKey(key))
                    {
                        var value = settings[key];
                        if (value is T)
                            return (T)value;
                        return defaultValue;
                    }
                    return defaultValue;
                }
                else
                    throw new NotImplementedException();
            }
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
                var settings = type.GetRuntimeProperty("ApplicationSettings").GetValue(null, null);
                var settingsType = settings.GetType();
                if ((bool)settingsType.GetRuntimeMethod("Contains", new Type[] { typeof(string) }).Invoke(settings, new object[] { key }))
                {
                    var value = settingsType.GetRuntimeProperty("Item").GetValue(settings, new object[] { key });
                    if (value is T)
                        return true;
                    return false;
                }
                return false;
            }
            else
            {
                type = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime");
                if (type != null)
                {
                    var settings = GetWinRtSettings(type, roaming);
                    if (settings.ContainsKey(key))
                    {
                        var value = settings[key];
                        if (value is T)
                            return true;
                        return false;
                    }
                    return false;
                }
                else
                    throw new NotImplementedException();
            }
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
                var settings = type.GetRuntimeProperty("ApplicationSettings").GetValue(null, null);
                var settingsType = settings.GetType();
                if ((bool)settingsType.GetRuntimeMethod("Contains", new Type[] { typeof(string) }).Invoke(settings, new object[] { key }))
                {
                    var value = (bool)settingsType.GetRuntimeMethod("Remove", new Type[] { typeof(string) }).Invoke(settings, new object[] { key });
                    if (value && save)
                        settingsType.GetRuntimeMethod("Save", new Type[] { }).Invoke(settings, null);
                    return save;
                }
                return false;
            }
            else
            {
                type = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime");
                if (type != null)
                {
                    var settings = GetWinRtSettings(type, roaming);
                    if (settings.ContainsKey(key))
                        return settings.Remove(key);
                    return false;
                }
                else
                    throw new NotImplementedException();
            }
        }
    }

#else

    //// TODO: Remove unused code => only SL5 and WP7.5 needed

    /// <summary>Provides methods to access and write settings to the isolated storage (works only for WP and WinRT). </summary>
    public static class ApplicationSettings
    {
        private static IDictionary<string, object> GetWinRtSettings(Type type, bool roaming)
        {
            var data = type.GetProperty("Current").GetValue(null, null);
            var settings = roaming
                ? data.GetType().GetProperty("RoamingSettings").GetValue(data, null)
                : data.GetType().GetProperty("LocalSettings").GetValue(data, null);
            return (IDictionary<string, object>)settings.GetType().GetProperty("Values").GetValue(settings, null);
        }

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
            {
                type = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime");
                if (type != null)
                {
                    var settings = GetWinRtSettings(type, roaming);
                    settings[key] = value;
                }
                else
                    throw new NotImplementedException();
            }
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
            {
                type = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime");
                if (type != null)
                {
                    var settings = GetWinRtSettings(type, roaming);
                    if (settings.ContainsKey(key))
                    {
                        var value = settings[key];
                        if (value is T)
                            return (T)value;
                        return defaultValue;
                    }
                    return defaultValue;
                }
                else
                    throw new NotImplementedException();
            }
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
            {
                type = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime");
                if (type != null)
                {
                    var settings = GetWinRtSettings(type, roaming);
                    if (settings.ContainsKey(key))
                    {
                        var value = settings[key];
                        if (value is T)
                            return true;
                        return false;
                    }
                    return false;
                }
                else
                    throw new NotImplementedException();
            }
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
            {
                type = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, ContentType=WindowsRuntime");
                if (type != null)
                {
                    var settings = GetWinRtSettings(type, roaming);
                    if (settings.ContainsKey(key))
                        return settings.Remove(key);
                    return false;
                }
                else
                    throw new NotImplementedException();
            }
        }
    }

#endif
}
