//-----------------------------------------------------------------------
// <copyright file="DataContractSerialization.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Threading.Tasks;
using MyToolkit.Serialization;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MyToolkit.Storage
{
    /// <summary>Provides methods to write and read complex setting objects from files.</summary>
    public static class StorageApplicationSettings
    {
        /// <summary>Serializes and writes a setting object in a file (uses the DataContractSerializer).</summary>
        /// <typeparam name="TValue">The type of the setting value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="roaming">If set to <c>true</c> the setting is synchronized between devices.</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns>The task.</returns>
        public static async Task SetSettingToFileAsync<TValue>(string key, TValue value, bool roaming = false, Type[] extraTypes = null)
        {
            var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting) :
                await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting);

            var xml = DataContractSerialization.Serialize(value, true, extraTypes); 
            await FileIO.WriteTextAsync(file, xml, UnicodeEncoding.Utf8);
        }

        /// <summary>Reads and deserializes a setting object from a file (uses the DataContractSerializer).</summary>
        /// <typeparam name="TValue">The type of the setting value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default setting value.</param>
        /// <param name="roaming">If set to <c>true</c> the setting is synchronized between devices.</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns>The task.</returns>
        public static async Task<TValue> GetSettingFromFileAsync<TValue>(string key, TValue defaultValue, bool roaming = false, Type[] extraTypes = null)
        {
            var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists) :
                await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists);

            var xml = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
            return !String.IsNullOrEmpty(xml) ? DataContractSerialization.Deserialize<TValue>(xml, extraTypes) : defaultValue;
        }

        /// <summary>Serializes and writes a setting object in a file (uses the XmlSerializer).</summary>
        /// <typeparam name="TValue">The type of the setting value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="roaming">If set to <c>true</c> the setting is synchronized between devices.</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns>The task.</returns>
        public static async Task SetSettingToXmlFileAsync<TValue>(string key, TValue value, bool roaming = false, Type[] extraTypes = null)
        {
            var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting) :
                await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.ReplaceExisting);

            var xml = XmlSerialization.Serialize(value, extraTypes);
            await FileIO.WriteTextAsync(file, xml, UnicodeEncoding.Utf8);
        }

        /// <summary>Reads and deserializes a setting object from a file (uses the XmlSerializer).</summary>
        /// <typeparam name="TValue">The type of the setting value.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default setting value.</param>
        /// <param name="roaming">If set to <c>true</c> the setting is synchronized between devices.</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns>The task.</returns>
        public static async Task<TValue> GetSettingFromXmlFileAsync<TValue>(string key, TValue defaultValue, bool roaming = false, Type[] extraTypes = null)
        {
            var file = roaming ? await ApplicationData.Current.RoamingFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists) :
                await ApplicationData.Current.LocalFolder.CreateFileAsync(key + ".settings", CreationCollisionOption.OpenIfExists);

            var xml = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);
            return !String.IsNullOrEmpty(xml) ? XmlSerialization.Deserialize<TValue>(xml, extraTypes) : defaultValue;
        }
    }
}

#endif