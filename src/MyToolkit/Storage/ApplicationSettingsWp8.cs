//-----------------------------------------------------------------------
// <copyright file="ApplicationSettingsWp8.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyToolkit.Storage
{
    internal class ApplicationSettingsWp8 : IApplicationSettings
    {
        private static readonly Type IsolatedStorageSettingsType;

        static ApplicationSettingsWp8()
        {
            IsolatedStorageSettingsType = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows");
            if (IsolatedStorageSettingsType == null)
                IsolatedStorageSettingsType = Type.GetType("System.IO.IsolatedStorage.IsolatedStorageSettings, System.Windows, Version=2.0.6.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
        }

        public static bool IsActive
        {
            get { return IsolatedStorageSettingsType != null; }
        }

        public bool CanRoam { get { return false; } }

        public void SetSetting<T>(string key, T value, bool roaming = false, bool save = false)
        {
            var settings = IsolatedStorageSettingsType.GetRuntimeProperty("ApplicationSettings").GetValue(null, null);
            var settingsType = settings.GetType();
            settingsType.GetRuntimeProperty("Item").SetValue(settings, value, new object[] { key });
            if (save)
                settingsType.GetRuntimeMethod("Save", new Type[] { }).Invoke(settings, null);
        }

        public T GetSetting<T>(string key, T defaultValue, bool roaming = false)
        {
            var settings = IsolatedStorageSettingsType.GetRuntimeProperty("ApplicationSettings").GetValue(null, null);
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

        public bool HasSetting<T>(string key, bool roaming = false)
        {
            var settings = IsolatedStorageSettingsType.GetRuntimeProperty("ApplicationSettings").GetValue(null, null);
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

        public bool RemoveSetting(string key, bool roaming = false, bool save = true)
        {
            var settings = IsolatedStorageSettingsType.GetRuntimeProperty("ApplicationSettings").GetValue(null, null);
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
    }
}