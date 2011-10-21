using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Threading;
using Ionic.Zlib;

// developed by Rico Suter (rsuter.com)

namespace MyToolkit.Network
{
	public class HttpPostFile
	{
		public HttpPostFile(string name, string filename, string path)
		{
			Name = name;
			Filename = System.IO.Path.GetFileName(filename);
			Path = path;
			CloseStream = true; 
		}

		public HttpPostFile(string name, string path)
		{
			Name = name;
			Filename = System.IO.Path.GetFileName(path);
			Path = path;
			CloseStream = true;
		}

		public HttpPostFile(string name, string filename, Stream stream, bool closeStream = true)
			: this(name, null)
		{
			Filename = System.IO.Path.GetFileName(filename);
			Stream = stream;
			CloseStream = closeStream;
		}

		public string Name { get; private set; }
		public string Filename { get; private set; }
		public string Path { get; private set; } // use Path OR Stream
		public Stream Stream { get; private set; }
		public bool CloseStream { get; private set; }

		public string ContentType { get; set; } // default: application/octet-stream
	}

	public class HttpPostRequest : HttpGetRequest
	{
		public HttpPostRequest(string uri) : base(uri)
		{
			Data = new Dictionary<string, string>();
			Files = new List<HttpPostFile>();
		}

		public Dictionary<string, string> Data { get; private set; }
		public List<HttpPostFile> Files { get; private set; }
	}

	public class HttpGetRequest
	{
		public HttpGetRequest(string uri)
			: this(uri, new Dictionary<string, string>())
		{ }

		public HttpGetRequest(string uri, Dictionary<string, string> query) 
		{
			URI = uri;
			Query = query;
			Cookies = new List<Cookie>();
			UseCache = true;
			Encoding = Encoding.UTF8;
			RequestGZIP = true; 
		}

		public string URI { get; private set; }
		public Dictionary<string, string> Query { get; private set; }
		public List<Cookie> Cookies { get; private set; }

		public bool UseCache { get; set; }
		public Encoding Encoding { get; set; }
		public bool RequestGZIP { get; set; }
	}

	public static class Http
	{
		public static void Post(string uri, Action<String, Exception> action, Dispatcher dispatcher)
		{
			Post(new HttpPostRequest(uri), (s, e) => dispatcher.BeginInvoke(() => action(s, e)));
		}

		public static void Post(string uri, Action<String, Exception> action)
		{
			Post(new HttpPostRequest(uri), action);
		}

		public static void Post(HttpPostRequest req, Action<String, Exception> action, Dispatcher dispatcher)
		{
			Post(req, (s, e) => dispatcher.BeginInvoke(() => action(s, e)));
		}

		public static void Post(HttpPostRequest req, Action<String, Exception> action)
		{
			try
			{
				var boundary = "";

				var queryString = GetQueryString(req.Query);
				var request = (HttpWebRequest)WebRequest.Create(req.URI + "?" + queryString);
			
				if (req.Files.Count == 0)
					request.ContentType = "application/x-www-form-urlencoded";
				else
				{
					boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
					request.ContentType = "multipart/form-data; boundary=" + boundary;
				}

				if (req.Cookies.Count > 0)
				{
					request.CookieContainer = new CookieContainer();
					foreach (var c in req.Cookies)
						request.CookieContainer.Add(request.RequestUri, c);
				}

				request.Method = "POST";
				if (req.RequestGZIP)
					request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";

				request.BeginGetRequestStream(delegate(IAsyncResult ar1)
				{
					try
					{
						var stream = request.EndGetRequestStream(ar1);
						if (req.Files.Count > 0)
							WritePostData(stream, boundary, req.Data, req.Files, req.Encoding);
						else
							WritePostData(stream, req.Data, req.Encoding);
						stream.Close();
						request.BeginGetResponse(r => ProcessResponse(r, request, req, action), request);
					}
					catch (Exception e)
					{
						action(null, e);
					}
				}, request);
			}
			catch (Exception e)
			{
				action(null, e);
			}
		}

		private static void WritePostData(Stream stream, Dictionary<string, string> postData, Encoding encoding)
		{
			var bytes = encoding.GetBytes(GetQueryString(postData));
			stream.Write(bytes, 0, bytes.Length);
		}

		private static void WritePostData(Stream stream, String boundary, Dictionary<String, String> postData, IEnumerable<HttpPostFile> files, Encoding encoding)
		{
			var boundarybytes = encoding.GetBytes("\r\n--" + boundary + "\r\n");

			var formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
			if (postData != null)
			{
				foreach (var tuple in postData)
				{
					stream.Write(boundarybytes, 0, boundarybytes.Length);

					var formitem = string.Format(formdataTemplate, tuple.Key, tuple.Value);
					var formitembytes = encoding.GetBytes(formitem);
					stream.Write(formitembytes, 0, formitembytes.Length);
				}
			}

			const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			foreach (var file in files)
			{
				stream.Write(boundarybytes, 0, boundarybytes.Length);

				var header = string.Format(headerTemplate, file.Name, file.Filename, file.ContentType ?? "application/octet-stream");
				var headerbytes = encoding.GetBytes(header);

				stream.Write(headerbytes, 0, headerbytes.Length);

				var fileStream = file.Stream;
				if (fileStream == null)
					fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);

				try
				{
					var buffer = new byte[1024];
					var bytesRead = 0;
					while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
						stream.Write(buffer, 0, bytesRead);
				}
				finally
				{
					if (file.CloseStream)
						fileStream.Close();
				}
			}

			boundarybytes = encoding.GetBytes("\r\n--" + boundary + "--\r\n");
			stream.Write(boundarybytes, 0, boundarybytes.Length);
		}

		public static void Get(string uri, Action<String, Exception> action, Dispatcher dispatcher)
		{
			Get(new HttpGetRequest(uri), (s, e) => dispatcher.BeginInvoke(() => action(s, e)));
		}

		public static void Get(string uri, Action<String, Exception> action)
		{
			Get(new HttpGetRequest(uri), action);
		}

		public static void Get(HttpGetRequest req, Action<String, Exception> action, Dispatcher dispatcher)
		{
			Get(req, (s, e) => dispatcher.BeginInvoke(() => action(s, e)));
		}

		public static void Get(HttpGetRequest req, Action<String, Exception> action)
		{
			try
			{
				HttpWebRequest request;
				if (req.Query != null)
				{
					if (!req.UseCache)
						req.Query["__dcachetime"] = DateTime.Now.Ticks.ToString(); // TODO auch im else, wenn kein query

					var queryString = GetQueryString(req.Query);
					if (req.URI.Contains("?"))
						request = (HttpWebRequest)WebRequest.Create(req.URI + "&" + queryString);
					else
						request = (HttpWebRequest)WebRequest.Create(req.URI + "?" + queryString);
				} else
					request = (HttpWebRequest)WebRequest.Create(req.URI);

				if (req.Cookies.Count > 0)
				{
					request.CookieContainer = new CookieContainer();
					foreach (var c in req.Cookies)
						request.CookieContainer.Add(request.RequestUri, c);
				}

				request.Method = "GET";
				if (req.RequestGZIP)
					request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
				request.BeginGetResponse(r => ProcessResponse(r, request, req, action), request);
			}
			catch (Exception e)
			{
				if (action != null)
					action(null, e);
			}
		}

		private static void ProcessResponse(IAsyncResult asyncResult, WebRequest request, HttpGetRequest req, Action<string, Exception> action)
		{
			try
			{
				var response = request.EndGetResponse(asyncResult);
				if (response.Headers[HttpRequestHeader.ContentEncoding] == "gzip")
					response = new GZipWebResponse(response); 
				using (var reader = new StreamReader(response.GetResponseStream(), req.Encoding))
				{
					var result = reader.ReadToEnd();
					if (action != null)
						action(result, null);
				}
			}
			catch (Exception e)
			{
				if (action != null)
					action(null, e);
			}
		}

		private static string GetQueryString(Dictionary<string, string> query)
		{
			var queryString = "";
			foreach (var p in query)
				queryString += Uri.EscapeDataString(p.Key) + "=" + (p.Value == null ? "" : Uri.EscapeDataString(p.Value)) + "&";
			return queryString.Trim('&');
		}
	}

	internal class GZipWebResponse : WebResponse
	{
		readonly WebResponse response;
		internal GZipWebResponse(WebResponse resp)
		{
			response = resp;
		}
		public override Stream GetResponseStream()
		{
			return new GZipStream(response.GetResponseStream(), CompressionMode.Decompress); 
		}

	    public override void Close()
	    {
	       
	    }

#if SILVERLIGHT
		public override long ContentLength
		{
			get { throw new NotImplementedException(); }
		}

		public override string ContentType
		{
			get { throw new NotImplementedException(); }
		}

		public override Uri ResponseUri
		{
			get { throw new NotImplementedException(); }
		}
#endif
	}
}
