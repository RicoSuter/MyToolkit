using System;
using System.Xml;

namespace MyToolkit.Environment
{
	public static class PhoneManifest
	{
		public static string GetAttribute(string attributeName)
		{
			try
			{
				const string appManifestName = "WMAppManifest.xml";
				const string appNodeName = "App";
				
				var settings = new XmlReaderSettings { XmlResolver = new XmlXapResolver() };
				using (var reader = XmlReader.Create(appManifestName, settings))
				{
					reader.ReadToDescendant(appNodeName);
					if (!reader.IsStartElement())
						return "";
					return reader.GetAttribute(attributeName);
				}
			}
			catch (Exception)
			{
				return "";
			}
		}
	}
}
