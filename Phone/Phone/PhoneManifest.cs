using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;

namespace MyToolkit.Phone
{
	public class PhoneManifest
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
