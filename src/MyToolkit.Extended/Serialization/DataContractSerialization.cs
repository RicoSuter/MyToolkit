//-----------------------------------------------------------------------
// <copyright file="DataContractSerialization.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MyToolkit.Utilities;

namespace MyToolkit.Serialization
{
    public static class DataContractSerialization
    {
#if !WP7 && !SL5

        private static readonly Dictionary<Type, List<Tuple<Type[], bool, DataContractSerializer>>> Serializers =
            new Dictionary<Type, List<Tuple<Type[], bool, DataContractSerializer>>>();

        /// <summary>Creates or retrieves a serializer for the given type and extra types. </summary>
        /// <typeparam name="T">The type to create the serialzer for. </typeparam>
        /// <param name="preserveReferences">Specifies whether to preserve references. </param>
        /// <param name="extraTypes">The extra types. </param>
        /// <returns>The <see cref="XmlSerializer"/>. </returns>
        public static DataContractSerializer CreateCachedSerializer<T>(bool preserveReferences = true, Type[] extraTypes = null)
        {
            var type = typeof(T);

            List<Tuple<Type[], bool, DataContractSerializer>> list;
            if (Serializers.TryGetValue(type, out list))
            {
                var tuple = list.SingleOrDefault(t => t.Item2 == preserveReferences && (t.Item1 == extraTypes || t.Item1.IsCopyOf(extraTypes)));
                if (tuple != null)
                    return tuple.Item3;

                var serializer = CreateSerializer<T>(preserveReferences, extraTypes);
                list.Add(new Tuple<Type[], bool, DataContractSerializer>(extraTypes, preserveReferences, serializer));
                return serializer;
            }
            else
            {
                var serializer = CreateSerializer<T>(preserveReferences, extraTypes);
                Serializers[type] = new List<Tuple<Type[], bool, DataContractSerializer>>
                {
                    new Tuple<Type[], bool, DataContractSerializer>(extraTypes, preserveReferences, serializer)
                };
                return serializer;
            }
        }

        /// <summary>Creates a serializer for the given type and extra types. </summary>
        /// <typeparam name="T">The type to create the serialzer for. </typeparam>
        /// <param name="preserveReferences">Specifies whether to preserve references. </param>
        /// <param name="extraTypes">The extra types. </param>
        /// <returns>The <see cref="XmlSerializer"/>. </returns>
        public static DataContractSerializer CreateSerializer<T>(bool preserveReferences = true, Type[] extraTypes = null)
        {
            var settings = new DataContractSerializerSettings();
            if (extraTypes != null)
                settings.KnownTypes = extraTypes;
            settings.PreserveObjectReferences = preserveReferences;
            return new DataContractSerializer(typeof(T), settings);
        }

#endif

#if WP7 || SL5

        /// <summary>Serializes an object to a XML string. </summary>
        /// <typeparam name="T">The type of the object to serialize. </typeparam>
        /// <param name="obj">The object to serialize. </param>
        /// <param name="preserveReferences">Specifies whether to preserve references. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <returns>The XML string. </returns>
        public static string Serialize<T>(T obj, bool preserveReferences = false, Type[] extraTypes = null)
        {
            if (preserveReferences)
                throw new ArgumentException("preserveReferences cannot be true (not supported)");

            var serializer = extraTypes != null ? new DataContractSerializer(obj.GetType(), extraTypes) : new DataContractSerializer(obj.GetType());
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                serializer.WriteObject(writer, obj);
                writer.Flush();
                return sb.ToString();
            }
        }

        /// <summary>Deserializes an object from a XML string. </summary>
        /// <typeparam name="T">The type of the resulting object. </typeparam>
        /// <param name="xml">The XML string. </param>
        /// <param name="extraTypes">The addional types. </param>
        /// <returns>The deserialized object. </returns>
        public static T Deserialize<T>(string xml, Type[] extraTypes = null)
        {
            var serializer = extraTypes != null ? new DataContractSerializer(typeof(T), extraTypes) : new DataContractSerializer(typeof(T));
            using (var reader = XmlReader.Create(new StringReader(xml)))
                return (T)serializer.ReadObject(reader);
        }

#else
#if WINRT

        /// <summary>Checks whether the given type can be serialized with the <see cref="DataContractSerializer"/>. </summary>
        public static bool CanSerialize(Type type)
        {
            if (type == typeof(String))
                return true;
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsClass)
                return typeInfo.GetCustomAttribute<DataContractAttribute>() != null;
            return true;
        }

        /// <summary>Checks whether the given generic type can be serialized with the <see cref="DataContractSerializer"/>. </summary>
        public static bool CanSerialize<T>()
        {
            return CanSerialize(typeof(T));
        }

#endif

        /// <summary>Serializes an object to a XML string. </summary>
        /// <typeparam name="T">The type of the object to serialize. </typeparam>
        /// <param name="obj">The object to serialize. </param>
        /// <param name="preserveReferences">Specifies whether to preserve references. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The XML string. </returns>
        public static string Serialize<T>(T obj, bool preserveReferences = true, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var reader = XmlWriter.Create(stringWriter))
                {
                    var serializer = useSerializerCache
                        ? CreateCachedSerializer<T>(preserveReferences, extraTypes)
                        : CreateSerializer<T>(preserveReferences, extraTypes);
                    serializer.WriteObject(reader, obj);
                }
                return stringWriter.ToString();
            }
        }

        /// <summary>Deserializes an object from a XML string. </summary>
        /// <typeparam name="T">The type of the resulting object. </typeparam>
        /// <param name="xml">The XML string. </param>
        /// <param name="extraTypes">The addional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The deserialized object. </returns>
        public static T Deserialize<T>(string xml, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            using (var stringReader = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    var serializer = useSerializerCache
                        ? CreateCachedSerializer<T>(true, extraTypes)
                        : CreateSerializer<T>(true, extraTypes);
                    return (T)serializer.ReadObject(reader);
                }
            }
        }

        //public static object Deserialize(string xml, Type type, Type[] extraTypes = null, bool useSerializerCache = true)
        //{
        //    using (var stringReader = new StringReader(xml))
        //    {
        //        using (var reader = XmlReader.Create(stringReader))
        //        {
        //            var serializer = useSerializerCache
        //                ? CreateCachedSerializer<T>(preserveReferences, extraTypes)
        //                : CreateSerializer<T>(preserveReferences, extraTypes);
        //            return serializer.ReadObject(reader);
        //        }
        //    }
        //}

#endif

        /// <summary>Asynchronously serializes an object to a XML string. </summary>
        /// <typeparam name="T">The type of the object to serialize. </typeparam>
        /// <param name="preserveReferences">Specifies whether to preserve references. </param>
        /// <param name="obj">The object to serialize. </param>
        /// <param name="extraTypes">The additional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The XML string. </returns>
        public static Task<string> SerializeAsync<T>(T obj, bool preserveReferences = true, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            var source = new TaskCompletionSource<string>();
            Task.Factory.StartNew(() =>
            {
                try
                {
#if WP7 || SL5
                    source.SetResult(Serialize(obj, preserveReferences, extraTypes));
#else
                    source.SetResult(Serialize(obj, preserveReferences, extraTypes, useSerializerCache));
#endif
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
        /// <param name="extraTypes">The addional types. </param>
        /// <param name="useSerializerCache">Specifies whether to cache the serializer (default: true). </param>
        /// <returns>The deserialized object. </returns>
        public static Task<T> DeserializeAsync<T>(string xml, Type[] extraTypes = null, bool useSerializerCache = true)
        {
            var source = new TaskCompletionSource<T>();
            Task.Factory.StartNew(() =>
            {
                try
                {
#if WP7 || SL5
                    source.SetResult(Deserialize<T>(xml, extraTypes));
#else
                    source.SetResult(Deserialize<T>(xml, extraTypes, useSerializerCache));
#endif
                    }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
            return source.Task;
        }
    }
}
