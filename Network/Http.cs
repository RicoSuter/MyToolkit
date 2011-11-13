using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;

#if SILVERLIGHT
using Ionic.Zlib;
#else
using System.IO.Compression;
#endif

#if !METRO
using System.Windows.Threading;
using MyToolkit.Utilities;

#endif

// developed by Rico Suter (http://rsuter.com), http://mytoolkit.codeplex.com

namespace MyToolkit.Network
{
	public static class Http
	{
		private static List<HttpResponse> pendingRequests;
		public static IEnumerable<HttpResponse> PendingRequests { get { return pendingRequests; } }
		
		public static void AbortAllRequests()
		{
			if (pendingRequests != null)
			{
				foreach (var r in pendingRequests)
					r.Abort();
			}
		}

		public static void AbortRequests(Func<HttpResponse, bool> abortPredicate)
		{
			if (pendingRequests != null)
			{
				foreach (var r in pendingRequests.Where(abortPredicate))
					r.Abort();
			}
		}

		#region GET

#if !METRO
		public static HttpResponse Get(string uri, Action<HttpResponse> action, Dispatcher dispatcher)
		{
			return Get(new HttpGetRequest(uri), r => dispatcher.BeginInvoke(() => action(r)));
		}

		public static HttpResponse Get(HttpGetRequest req, Action<HttpResponse> action, Dispatcher dispatcher)
		{
			return Get(req, r => dispatcher.BeginInvoke(() => action(r)));
		}
#endif

		public static HttpResponse Get(string uri, Action<HttpResponse> action)
		{
			return Get(new HttpGetRequest(uri), action);
		}

		public static HttpResponse Get(HttpGetRequest req, Action<HttpResponse> action)
		{
			var response = new HttpResponse(req);
			try
			{
				HttpWebRequest request;
				if (req.Query != null)
				{
					if (!req.UseCache)
						req.Query["__dcachetime"] = DateTime.Now.Ticks.ToString(); // TODO auch im else, wenn kein query

					var queryString = GetQueryString(req.Query);
					if (req.Uri.Contains("?"))
						request = (HttpWebRequest)WebRequest.Create(req.Uri + "&" + queryString);
					else
						request = (HttpWebRequest)WebRequest.Create(req.Uri + "?" + queryString);
				}
				else
					request = (HttpWebRequest)WebRequest.Create(req.Uri);
				response.WebRequest = request;

				if (req.Cookies.Count > 0)
				{
					request.CookieContainer = new CookieContainer();
					foreach (var c in req.Cookies)
						request.CookieContainer.Add(request.RequestUri, c);
				}

				request.Method = "GET";
				if (req.ContentType != null)
					request.ContentType = req.ContentType; 

#if USE_GZIP
				if (req.RequestGZIP)
					request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
#endif

				request.BeginGetResponse(r => ProcessResponse(r, request, response, action), request);
			}
			catch (Exception e)
			{
				response.Exception = e;
				if (action != null)
					action(response);
			}

			if (pendingRequests == null)
				pendingRequests = new List<HttpResponse>();
			pendingRequests.Add(response);
			return response;
		}

		#endregion

		#region POST

#if !METRO
		public static HttpResponse Post(string uri, Action<HttpResponse> action, Dispatcher dispatcher)
		{
			return Post(new HttpPostRequest(uri), r => dispatcher.BeginInvoke(() => action(r)));
		}

		public static HttpResponse Post(HttpPostRequest req, Action<HttpResponse> action, Dispatcher dispatcher)
		{
			return Post(req, r => dispatcher.BeginInvoke(() => action(r)));
		}
#endif

		public static HttpResponse Post(string uri, Action<HttpResponse> action)
		{
			return Post(new HttpPostRequest(uri), action);
		}

		public static HttpResponse Post(HttpPostRequest req, Action<HttpResponse> action)
		{
			var response = new HttpResponse(req);
			try
			{
				var boundary = "";
				var queryString = GetQueryString(req.Query);

				var request = (HttpWebRequest)WebRequest.Create(req.Uri + "?" + queryString);
				response.WebRequest = request; 

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
				if (req.ContentType != null)
					request.ContentType = req.ContentType; 

#if USE_GZIP
				if (req.RequestGZIP)
					request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
#endif

				request.BeginGetRequestStream(delegate(IAsyncResult ar1)
				{
					try
					{
						using (var stream = request.EndGetRequestStream(ar1))
						{
							if (req.Files.Count > 0)
								WritePostData(stream, boundary, req);
							else
								WritePostData(stream, req);
						}
						request.BeginGetResponse(r => ProcessResponse(r, request, response, action), request);
					}
					catch (Exception e)
					{
						response.Exception = e;
						if (action != null)
							action(response);
					}
				}, request);
			}
			catch (Exception e)
			{
				response.Exception = e;
				if (action != null)
					action(response);
			}

			if (pendingRequests == null)
				pendingRequests = new List<HttpResponse>();
			pendingRequests.Add(response);
			return response;
		}

		private static void WritePostData(Stream stream, HttpPostRequest request)
		{
			var bytes = request.RawData ?? request.Encoding.GetBytes(GetQueryString(request.Data));
			stream.Write(bytes, 0, bytes.Length);
		}

		private static void WritePostData(Stream stream, String boundary, HttpPostRequest request)
		{
			var boundarybytes = request.Encoding.GetBytes("\r\n--" + boundary + "\r\n");

			var formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
			if (request.RawData != null)
				throw new Exception("RawData not allowed if uploading files");

			foreach (var tuple in request.Data)
			{
				stream.Write(boundarybytes, 0, boundarybytes.Length);

				var formitem = string.Format(formdataTemplate, tuple.Key, tuple.Value);
				var formitembytes = request.Encoding.GetBytes(formitem);
				stream.Write(formitembytes, 0, formitembytes.Length);
			}

			const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			foreach (var file in request.Files)
			{
				stream.Write(boundarybytes, 0, boundarybytes.Length);

				var header = string.Format(headerTemplate, file.Name, file.Filename, file.ContentType ?? "application/octet-stream");
				var headerbytes = request.Encoding.GetBytes(header);

				stream.Write(headerbytes, 0, headerbytes.Length);

				var fileStream = file.Stream;
#if !METRO
				if (fileStream == null)
					fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
#endif

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
						fileStream.Dispose();
				}
			}

			boundarybytes = request.Encoding.GetBytes("\r\n--" + boundary + "--\r\n");
			stream.Write(boundarybytes, 0, boundarybytes.Length);
		}

		#endregion

		private static void ProcessResponse(IAsyncResult asyncResult, WebRequest request, HttpResponse resp, Action<HttpResponse> action)
		{
			if (pendingRequests != null && pendingRequests.Contains(resp))
				pendingRequests.Remove(resp);

			try
			{
				var response = request.EndGetResponse(asyncResult);
				var origResponse = response;

#if USE_GZIP
				if (response.Headers[HttpRequestHeader.ContentEncoding] == "gzip")
					response = new GZipWebResponse(response); 
#endif

				using (response)
				{
					using (var reader = new StreamReader(response.GetResponseStream(), resp.Request.Encoding))
					{
						resp.Response = reader.ReadToEnd();
						if (origResponse.Headers.AllKeys.Contains("Set-Cookie"))
						{
							var cookies = origResponse.Headers["Set-Cookie"];
							var index = cookies.IndexOf(';');
							if (index != -1)
							{
								foreach (var c in HttpUtilityExtensions.ParseQueryString(cookies.Substring(0, index)))
									resp.Cookies.Add(new Cookie(c.Key, c.Value));
							}
						}
					}
				}
				if (action != null)
					action(resp);
			}
			catch (Exception e)
			{
				resp.Exception = e; 
				if (action != null)
					action(resp);
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

#if USE_GZIP
	internal class GZipWebResponse : WebResponse, IDisposable
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

#if METRO
		protected override void Dispose(bool disposing)
		{
            response.Dispose();
			base.Dispose(disposing);
		}
#else

		void IDisposable.Dispose()
		{
			Close();
		}

		public override void Close()
		{
			response.Close();
		}
#endif

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
#endif
}
