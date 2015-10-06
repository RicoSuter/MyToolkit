//-----------------------------------------------------------------------
// <copyright file="XmlSerialization.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MyToolkit.IO;
using MyToolkit.Utilities;

namespace MyToolkit.Serialization
{
    /// <summary>Provides methods to serialize and deserialize objects to XML. </summary>
    public static class XmlSerialization
    {
        private static readonly Dictionary<Type, List<Tuple<Type[], XmlSerializer>>> Serializers =
            new Dictionary<Type, List<Tuple<Type[], XmlSerializer>>>();

        /// <summary>Creates or retrieves a serializer for the given type and extra types. </summary>
        /// <typeparam name="T">The type to create the serializer for. </typeparam>
        /// <param name="extraTypes">The extra types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The <see cref="XmlSerializer"/>. </returns>
        public static XmlSerializer CreateSerializer<T>(Type[] extraTypes = null, bool useSerializerCache = true)
        {
            var type = typeof(T);
            if (!useSerializerCache)
                return extraTypes == null ? new XmlSerializer(type) : new XmlSerializer(type, extraTypes);

            List<Tuple<Type[], XmlSerializer>> list;
            if (Serializers.TryGetValue(type, out list))
            {
                var tuple = list.SingleOrDefault(types => types.Item1 == extraTypes || types.Item1.IsCopyOf(extraTypes));
                if (tuple != null)
                    return tuple.Item2;

                var serializer = extraTypes == null ? new XmlSerializer(type) : new XmlSerializer(type, extraTypes);
                list.Add(new Tuple<Type[], XmlSerializer>(extraTypes, serializer));
                return serializer;
            }
            else
            {
                var serializer = extraTypes == null ? new XmlSerializer(type) : new XmlSerializer(type, extraTypes);
                Serializers[type] = new List<Tuple<Type[], XmlSerializer>>
                {
                    new Tuple<Type[], XmlSerializer>(extraTypes, serializer)
                };
                return serializer;
            }
        }

        /// <summary>Serializes an object to a XML string. </summary>
        /// <typeparam name="T">The type of the object to serialize. </typeparam>
        /// <param name="obj">The object to serialize. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The XML string. </returns>
        public static string Serialize<T>(T obj, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            using (var sw = new Utf8StringWriter())
            {
                var serializer = CreateSerializer<T>(extraTypes, useSerializerCache);
                serializer.Serialize(sw, obj);
                return sw.ToString();
            }
        }

        /// <summary>Deserializes an object from a XML string. </summary>
        /// <typeparam name="T">The type of the resulting object. </typeparam>
        /// <param name="xml">The XML string. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The deserialized object. </returns>
        public static T Deserialize<T>(string xml, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            using (var sw = new StringReader(xml))
            {
                var serializer = CreateSerializer<T>(extraTypes, useSerializerCache);
                return (T)serializer.Deserialize(sw);
            }
        }

        /// <summary>Serializes a dictionary to a XML string. </summary>
        /// <typeparam name="TKey">The dictionary key type. </typeparam>
        /// <typeparam name="TValue">The dictionary value type. </typeparam>
        /// <param name="dictionary">The dictionary to serialize. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The XML string. </returns>
        public static string SerializeDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            return Serialize(dictionary.Select(p => new KeyValuePair<TKey, TValue>(p.Key, p.Value)).ToList());
        }

        /// <summary>Deserializes a dictionary from a XML string. </summary>
        /// <typeparam name="TKey">The dictionary key type. </typeparam>
        /// <typeparam name="TValue">The dictionary value type. </typeparam>
        /// <param name="xml">The XML string. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The deserialized object. </returns>
        public static Dictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(string xml, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            return Deserialize<List<KeyValuePair<TKey, TValue>>>(xml, extraTypes, useSerializerCache)
                .ToDictionary(p => p.Key, p => p.Value);
        }

        /// <summary>Asynchronously serializes an object to a XML string. </summary>
        /// <typeparam name="T">The type of the object to serialize. </typeparam>
        /// <param name="obj">The object to serialize. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The XML string. </returns>
        public static Task<string> SerializeAsync<T>(T obj, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            var source = new TaskCompletionSource<string>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    source.SetResult(Serialize(obj, extraTypes, useSerializerCache));
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            return source.Task;
        }

        /// <summary>Asynchronously deserializes an object from a XML string. </summary>
        /// <typeparam name="T">The type of the resulting object. </typeparam>
        /// <param name="xml">The XML string. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The deserialized object. </returns>
        public static Task<T> DeserializeAsync<T>(string xml, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            var source = new TaskCompletionSource<T>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    source.SetResult(Deserialize<T>(xml, extraTypes, useSerializerCache));
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            return source.Task;
        }

        /// <summary>Asynchronously serializes an object to a XML string. </summary>
        /// <typeparam name="TKey">The dictionary key type. </typeparam>
        /// <typeparam name="TValue">The dictionary value type. </typeparam>
        /// <param name="obj">The object to serialize. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The XML string. </returns>
        public static Task<string> SerializeDictionaryAsync<TKey, TValue>(Dictionary<TKey, TValue> obj, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            var source = new TaskCompletionSource<string>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    source.SetResult(SerializeDictionary(obj, extraTypes, useSerializerCache));
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            return source.Task;
        }

        /// <summary>Asynchronously deserializes an object from a XML string. </summary>
        /// <typeparam name="TKey">The dictionary key type. </typeparam>
        /// <typeparam name="TValue">The dictionary value type. </typeparam>
        /// <param name="xml">The XML string. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The deserialized object. </returns>
        public static Task<Dictionary<TKey, TValue>> DeserializeDictionaryAsync<TKey, TValue>(string xml, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            var source = new TaskCompletionSource<Dictionary<TKey, TValue>>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    source.SetResult(DeserializeDictionary<TKey, TValue>(xml, extraTypes, useSerializerCache));
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            return source.Task;
        }

        /// <summary>Used only for serialization</summary>
        public class KeyValuePair<TKey, TValue>
        {
            /// <summary>Initializes a new instance of the <see cref="KeyValuePair{TKey, TValue}"/> class.</summary>
            public KeyValuePair() { }

            /// <summary>Initializes a new instance of the <see cref="KeyValuePair{TKey, TValue}"/> class.</summary>
            /// <param name="key">The key.</param>
            /// <param name="value">The value.</param>
            public KeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            /// <summary>Gets or sets the key.</summary>
            public TKey Key { get; set; }

            /// <summary>Gets or sets the value.</summary>
            public TValue Value { get; set; }
        }
    }
}
