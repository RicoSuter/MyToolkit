using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

#if WINRT
using Windows.Data.Xml.Dom;
#elif WPF
using System.Xml;
#endif

#if !SL4
using System.Threading.Tasks;
#endif

namespace MyToolkit.Serialization
{
	public sealed class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding { get { return Encoding.UTF8; } }
	}

	public static class XmlSerialization
	{
		public static string Serialize(object obj, Type[] extraTypes = null)
		{
			using (var sw = new Utf8StringWriter())
			{
				var type = obj.GetType();
				var serializer = extraTypes == null ? new XmlSerializer(type) : new XmlSerializer(type, extraTypes);
				serializer.Serialize(sw, obj);
				return sw.ToString();
			}
		}

		public static T Deserialize<T>(string xml, Type[] extraTypes = null)
		{
			using (var sw = new StringReader(xml))
			{
				var serializer = extraTypes == null ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), extraTypes);
				return (T)serializer.Deserialize(sw);
			}
		}

#if !SL4

		public static Task<string> SerializeAsync(object obj, Type[] extraTypes = null)
		{
			var source = new TaskCompletionSource<string>();
			Task.Factory.StartNew(() =>
			{
				try
				{
					source.SetResult(Serialize(obj, extraTypes));
				}
				catch (Exception ex)
				{
					source.SetException(ex);
				}
			}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
			return source.Task;
		}

		public static Task<T> DeserializeAsync<T>(string xml, Type[] extraTypes = null)
		{
			var source = new TaskCompletionSource<T>();
			Task.Factory.StartNew(() =>
			{
				try
				{
					source.SetResult(Deserialize<T>(xml, extraTypes));
				}
				catch (Exception ex)
				{
					source.SetException(ex);
				}
			}, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
			return source.Task;
		}

#endif

#if WINRT

        public static string XmlEscape(string unescaped)
        {
            var doc = new XmlDocument();
            var node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerText;
        }

        public static string XmlUnescape(string escaped)
        {
            var doc = new XmlDocument();
            var node = doc.CreateElement("root");
            node.InnerText = escaped;
            return node.InnerText;
        }

#elif !SL4 && !WP7 && !WP8 && !SL5

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
