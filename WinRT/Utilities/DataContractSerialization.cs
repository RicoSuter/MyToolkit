using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace MyToolkit.Utilities
{
	public static class DataContractSerialization
	{
		public static string Serialize(object obj, bool preserveReferences)
		{
			using (var stringWriter = new StringWriter())
			{
				using (var reader = XmlWriter.Create(stringWriter))
				{
					var settings = new DataContractSerializerSettings();
					settings.PreserveObjectReferences = preserveReferences;
					var serializer = new DataContractSerializer(obj.GetType(), settings);
					serializer.WriteObject(reader, obj);
				}
				return stringWriter.ToString();
			}
		}

		public static T Deserialize<T>(string xml)
		{
			using (var stringReader = new StringReader(xml))
			{
				using (var reader = XmlReader.Create(stringReader))
				{
					var serializer = new DataContractSerializer(typeof(T));
					return (T)serializer.ReadObject(reader);
				}
			}
		}

		public static bool CanSerialize(Type type)
		{
			var typeInfo = type.GetTypeInfo(); 
			if (typeInfo.IsClass)
				return typeInfo.GetCustomAttribute<DataContractAttribute>() != null;
			return true; 
		}

		public static bool CanSerialize<T>()
		{
			return CanSerialize(typeof (T));
		}
	}
}
