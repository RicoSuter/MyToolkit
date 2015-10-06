//-----------------------------------------------------------------------
// <copyright file="ApplicationSettingsWinRt.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyToolkit.Storage
{
    internal class ApplicationSettingsWinRt : IApplicationSettings
    {
        private static readonly Type ApplicationDataType;

        static ApplicationSettingsWinRt()
        {
            ApplicationDataType = Type.GetType("Windows.Storage.ApplicationData, Windows.Storage, " +
                                               "Version=255.255.255.255, Culture=neutral, PublicKeyToken=null, " +
                                               "ContentType=WindowsRuntime");
        }

        public static bool IsActive
        {
            get { return ApplicationDataType != null; }
        }

        public bool CanRoam { get { return true; } }

        public void SetSetting<T>(string key, T value, bool roaming, bool save)
        {
            var settings = GetWinRtSettings(ApplicationDataType, roaming);
            settings[key] = value;
        }

        public T GetSetting<T>(string key, T defaultValue, bool roaming = false)
        {
            var settings = GetWinRtSettings(ApplicationDataType, roaming);
            if (settings.ContainsKey(key))
            {
                var value = settings[key];
                if (value is T)
                    return (T)value;
                return defaultValue;
            }
            return defaultValue;
        }

        public bool HasSetting<T>(string key, bool roaming = false)
        {
            var settings = GetWinRtSettings(ApplicationDataType, roaming);
            if (settings.ContainsKey(key))
            {
                var value = settings[key];
                if (value is T)
                    return true;
                return false;
            }
            return false;
        }

        public bool RemoveSetting(string key, bool roaming = false, bool save = true)
        {
            var settings = GetWinRtSettings(ApplicationDataType, roaming);
            if (settings.ContainsKey(key))
                return settings.Remove(key);
            return false;
        }

        private static IDictionary<string, object> GetWinRtSettings(Type type, bool roaming)
        {
            var data = type.GetRuntimeProperty("Current").GetValue(null, null);
            var settings = roaming
                ? data.GetType().GetRuntimeProperty("RoamingSettings").GetValue(data, null)
                : data.GetType().GetRuntimeProperty("LocalSettings").GetValue(data, null);
            return (IDictionary<string, object>)settings.GetType().GetRuntimeProperty("Values").GetValue(settings, null);
        }
    }
}