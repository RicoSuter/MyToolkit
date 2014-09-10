//-----------------------------------------------------------------------
// <copyright file="ApplicationSettingsWpf.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MyToolkit.Serialization;

namespace MyToolkit.Storage
{
    internal class ApplicationSettingsWpf : IApplicationSettings
    {
        private static readonly Type DirectoryType;
        private static readonly Type FileType;

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

        private string GetConfigurationPath()
        {
            var enumType = typeof(Environment).GetTypeInfo().GetDeclaredNestedType("SpecialFolder").AsType();
            var appDataDirectory = (string)typeof(Environment).GetRuntimeMethod("GetFolderPath", new[] { enumType }).Invoke(
                null, new object[] { Enum.Parse(enumType, "ApplicationData") });

            var assembly = (Assembly)typeof(Assembly).GetRuntimeMethod("GetEntryAssembly", new Type[] { }).Invoke(null, null) ??
                           (Assembly)typeof(Assembly).GetRuntimeMethod("GetExecutingAssembly", new Type[] { }).Invoke(null, null);

            var executableName = Path.GetFileNameWithoutExtension((string)assembly.GetType().GetRuntimeProperty("CodeBase").GetValue(assembly));
            var filePath = Path.Combine(appDataDirectory, executableName + "/MtConfig.xml");

            var directoryPath = Path.GetDirectoryName(filePath);
            if (directoryPath != null && !(bool)DirectoryType.GetRuntimeMethod("Exists", new[] { typeof(string) }).Invoke(null, new object[] { directoryPath }))
                DirectoryType.GetRuntimeMethod("CreateDirectory", new[] { typeof(string) }).Invoke(null, new object[] { directoryPath });

            return filePath;
        }

        private Dictionary<string, object> GetConfiguration()
        {
            var path = GetConfigurationPath();
            if (!(bool)FileType.GetRuntimeMethod("Exists", new Type[] { typeof(string) }).Invoke(null, new object[] { path }))
                return new Dictionary<string, object>();

            var data = (string)FileType.GetRuntimeMethod("ReadAllText", new[] { typeof(string), typeof(Encoding) })
                .Invoke(null, new object[] { path, Encoding.UTF8 });
            return XmlSerialization.DeserializeDictionary<string, object>(data);
        }

        private void SetConfiguration(Dictionary<string, object> config)
        {
            var data = XmlSerialization.SerializeDictionary(config);
            FileType.GetRuntimeMethod("WriteAllText", new[] { typeof(string), typeof(string), typeof(Encoding) })
                .Invoke(null, new object[] { GetConfigurationPath(), data, Encoding.UTF8 });
        }

        public void SetSetting<T>(string key, T value, bool roaming, bool save)
        {
            var config = GetConfiguration();
            config[key] = value;
            SetConfiguration(config);
        }

        public T GetSetting<T>(string key)
        {
            return GetSetting(key, default(T), false);
        }

        public T GetSetting<T>(string key, T defaultValue, bool roaming)
        {
            var config = GetConfiguration();
            if (config.ContainsKey(key))
                return (T)config[key];
            return defaultValue;
        }

        public bool HasSetting<T>(string key, bool roaming)
        {
            var config = GetConfiguration();
            return config.ContainsKey(key);
        }

        public bool RemoveSetting(string key, bool roaming, bool save)
        {
            var config = GetConfiguration();
            var removed = config.ContainsKey(key);
            if (config.ContainsKey(key))
                config.Remove(key);
            SetConfiguration(config);
            return removed;
        }

        public bool CanRoam { get { return false; } }
    }
}
