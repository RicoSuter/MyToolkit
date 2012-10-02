using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace MyToolkit.Utilities
{
	public static class DataContractSerialization
	{
		public static string Serialize(object obj)
		{
			using (var stringWriter = new StringWriter())
			{
				using (var reader = XmlWriter.Create(stringWriter))
				{
					var serializer = new DataContractSerializer(obj.GetType());
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
	}
}
