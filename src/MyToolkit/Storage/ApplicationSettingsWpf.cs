//-----------------------------------------------------------------------
// <copyright file="ApplicationSettingsWpf.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MyToolkit.Events;
using MyToolkit.Serialization;
using MyToolkit.Utilities;

namespace MyToolkit.Storage
{
    internal class ApplicationSettingsWpf : IApplicationSettings
    {
        private static readonly Type DirectoryType;
        private static readonly Type FileType;

        private bool _eventRegistered = false;
        private Dictionary<string, object> _configuration = null;
        private bool _isDirty = false; 

        static ApplicationSettingsWpf()
        {
            DirectoryType = Type.GetType("System.IO.Directory");
            FileType = Type.GetType("System.IO.File");
        }

        public static bool IsActive
        {
            get
            {
                return Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, " +
                                    "Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35") != null;
            }
        }

        private Dictionary<string, object> Configuration
        {
            get
            {
                if (_configuration == null)
                    _configuration = ReadConfiguration();
                return _configuration;
            }
        }

        public void SetSetting<T>(string key, T value, bool roaming, bool save)
        {
            lock (this)
            {
                Configuration[key] = value;
                _isDirty = true;

                if (save)
                    WriteConfiguration();
                else
                    RegisterExitEvent();
            }
        }

        public T GetSetting<T>(string key)
        {
            return GetSetting(key, default(T), false);
        }

        public T GetSetting<T>(string key, T defaultValue, bool roaming)
        {
            lock (this)
            {
                if (Configuration.ContainsKey(key))
                    return (T)Configuration[key];
                return defaultValue;
            }
        }

        public bool HasSetting<T>(string key, bool roaming)
        {
            lock (this)
                return Configuration.ContainsKey(key);
        }

        public bool RemoveSetting(string key, bool roaming, bool save)
        {
            lock (this)
            {
                var removed = Configuration.ContainsKey(key);

                if (Configuration.ContainsKey(key))
                {
                    Configuration.Remove(key);
                    _isDirty = true;
                }

                if (save)
                    WriteConfiguration();
                else
                    RegisterExitEvent();

                return removed;
            }
        }

        public bool CanRoam
        {
            get { return false; }
        }

        private void RegisterExitEvent()
        {
            if (!_eventRegistered)
            {
                var property = Type.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain");
                var currentDomain = property.GetValue(null);

                EventUtilities.RegisterEvent(currentDomain, "ProcessExit", (sender, args) => WriteConfiguration());

                _eventRegistered = true;
            }
        }

        private string GetConfigurationPath()
        {
            var enumType = typeof(Environment).GetTypeInfo().GetDeclaredNestedType("SpecialFolder").AsType();
            var appDataDirectory = (string)typeof(Environment).GetRuntimeMethod("GetFolderPath", new[] { enumType }).Invoke(
                null, new object[] { Enum.Parse(enumType, "ApplicationData") });

            var assembly = (Assembly)typeof(Assembly).GetRuntimeMethod("GetEntryAssembly", new Type[] { }).Invoke(null, null);
            if (assembly == null)
                assembly = (Assembly)typeof(Assembly).GetRuntimeMethod("GetExecutingAssembly", new Type[] { }).Invoke(null, null);

            var executableName = Path.GetFileNameWithoutExtension((string)assembly.GetType().GetRuntimeProperty("CodeBase").GetValue(assembly));
            var filePath = Path.Combine(appDataDirectory, executableName + "/MtConfig.xml");

            var directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath != null && !(bool)DirectoryType.GetRuntimeMethod("Exists", new[] { typeof(string) }).Invoke(null, new object[] { directoryPath }))
                DirectoryType.GetRuntimeMethod("CreateDirectory", new[] { typeof(string) }).Invoke(null, new object[] { directoryPath });

            return filePath;
        }

        private Dictionary<string, object> ReadConfiguration()
        {
            var path = GetConfigurationPath();
            if (!(bool)FileType.GetRuntimeMethod("Exists", new Type[] { typeof(string) }).Invoke(null, new object[] { path }))
                return new Dictionary<string, object>();

            var data = (string)FileType.GetRuntimeMethod("ReadAllText", new[] { typeof(string), typeof(Encoding) })
                .Invoke(null, new object[] { path, Encoding.UTF8 });

            return XmlSerialization.DeserializeDictionary<string, object>(data);
        }

        private void WriteConfiguration()
        {
            if (_isDirty)
            {
                var data = XmlSerialization.SerializeDictionary(Configuration);

                FileType.GetRuntimeMethod("WriteAllText", new[] { typeof(string), typeof(string), typeof(Encoding) })
                    .Invoke(null, new object[] { GetConfigurationPath(), data, Encoding.UTF8 });

                _isDirty = false;
            }
        }
    }
}
