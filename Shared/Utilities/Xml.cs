using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MyToolkit.Utilities
{
	public sealed class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding { get { return Encoding.UTF8; } }
	}

	public static class Xml
	{
		public static string Serialize(object obj)
		{
			using (var sw = new StringWriter()) 
			{
				var serializer = new XmlSerializer(obj.GetType());
				serializer.Serialize(sw, obj);
				return sw.ToString();
			}
		}

		public static T Deserialize<T>(string xml)
		{
			using (var sw = new StringReader(xml))
			{
				var serializer = new XmlSerializer(typeof(T));
				return (T)serializer.Deserialize(sw);
			}
		}

#if !METRO && !SILVERLIGHT
		// TODO (beta) implement for metro => should work in final version
		public static string XmlEscape(string unescaped)
		{
			var doc = new XmlDocument();
			var node = doc.CreateElement("root");
			node.InnerText = unescaped;
			return node.InnerXml;
		}

		public static string XmlUnescape(string escaped)
		{
			var doc = new XmlDocument();
			var node = doc.CreateElement("root");
			node.InnerXml = escaped;
			return node.InnerText;
		}
#endif
	}
}
