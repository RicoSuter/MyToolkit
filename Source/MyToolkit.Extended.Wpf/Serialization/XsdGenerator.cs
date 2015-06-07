using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MyToolkit.Serialization
{
	public static class XsdGenerator
	{
		private static void AttachXmlAttributes(XmlAttributeOverrides xao, Type t)
		{
			var types = new List<Type>();
			AttachXmlAttributes(xao, types, t);
		}

		private static void AttachXmlAttributes(XmlAttributeOverrides xao, List<Type> all, Type t)
		{
			if (all.Contains(t))
				return;
			
			all.Add(t);

			var list1 = GetAttributeList(t.GetCustomAttributes(false));
			xao.Add(t, list1);

			foreach (var prop in t.GetProperties())
			{
				var list2 = GetAttributeList(prop.GetCustomAttributes(false));
				xao.Add(t, prop.Name, list2);
				AttachXmlAttributes(xao, all, prop.PropertyType);
			}
		}

		private static XmlAttributes GetAttributeList(object[] attributes)
		{
			var list = new XmlAttributes();
			foreach (var attribute in attributes)
			{
				var type = attribute.GetType();
				if (type.Name == "XmlAttributeAttribute") 
					list.XmlAttribute = (XmlAttributeAttribute)attribute;
				else if (type.Name == "XmlArrayAttribute")
					list.XmlArray = (XmlArrayAttribute)attribute;
				else if (type.Name == "XmlArrayItemAttribute") 
					list.XmlArrayItems.Add((XmlArrayItemAttribute)attribute);

			}
			return list;
		}

		public static string GetSchema<T>()
		{
			var xao = new XmlAttributeOverrides();
			AttachXmlAttributes(xao, typeof(T));

			var importer = new XmlReflectionImporter(xao);
			var schemas = new XmlSchemas();
			var exporter = new XmlSchemaExporter(schemas);
			var map = importer.ImportTypeMapping(typeof(T));
			exporter.ExportTypeMapping(map);

			using (var ms = new MemoryStream())
			{
				schemas[0].Write(ms);
				ms.Position = 0;
				return new StreamReader(ms).ReadToEnd();
			}
		}
	}
}
