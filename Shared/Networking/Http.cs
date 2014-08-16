using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Linq;
using MyToolkit.Utilities;

#if SL4 || SL5
using Ionic.Zlib;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
#if SL5
using System.Threading.Tasks;
#endif
#elif WP7
using Ionic.Zlib;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
#elif WP8
using Ionic.Zlib;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
#elif WPF
using System.IO.Compression;
using System.Windows.Threading;
using System.Threading.Tasks;
#elif WINRT
using System.IO.Compression;
using Windows.Storage;
using System.Threading.Tasks;
#endif

// developed by Rico Suter (http://rsuter.com), http://mytoolkit.codeplex.com
namespace MyToolkit.Networking
{
	public class HttpStatusException : Exception
	{
		public HttpStatusException() { }
		public HttpStatusException(string message) : base(message) { }

		public HttpResponse Result { get; set; }
		public WebException WebException { get; set; }
	}

	public class AuthenticatedUri : Uri
	{
		public AuthenticatedUri(string uriString, string username, string password)
			: base(uriString)
		{
			UserName = username;
			Password = password;
		}

		public AuthenticatedUri(string uriString, UriKind uriKind, string username, string password)
			: base(uriString, uriKind)
		{
			UserName = username;
			Password = password;
		}

		public AuthenticatedUri(Uri baseUri, string relativeUri, string username, string password)
			: base(baseUri, relativeUri)
		{
			UserName = username;
			Password = password;
		}

		public AuthenticatedUri(Uri baseUri, Uri relativeUri, string username, string password)
			: base(baseUri, relativeUri)
		{
			UserName = username;
			Password = password;
		}

		public string UserName { get; set; }
		public string Password { get; set; }

		public ICredentials Credentials
		{
			get { return UserName == null ? null : new NetworkCredential(UserName, Password); }
		}
	}

	public static class Http
	{
		private static readonly List<HttpResponse> pendingRequests = new List<HttpResponse>();		
		public static void AbortAllRequests()
		{
			lock (pendingRequests)
			{
				foreach (var r in pendingRequests)
					r.Abort();
			}
		}

		public static void AbortRequests(Func<HttpResponse, bool> abortPredicate)
		{
			lock (pendingRequests)
			{
				foreach (var r in pendingRequests.Where(abortPredicate))
					r.Abort();
			}
		}

		public static int PendingRequestCount
		{
			get
			{
				lock (pendingRequests)
					return pendingRequests.Count;
			}
		}

		private static HttpWebRequest CreateRequest(HttpGetRequest req)
		{
			var queryString = GetQueryString(req.Query);

			HttpWebRequest request = null;
			if (string.IsNullOrEmpty(queryString))
				request = (HttpWebRequest)WebRequest.Create(req.Uri.AbsoluteUri);
			else if (req.Uri.AbsoluteUri.Contains("?"))
				request = (HttpWebRequest)WebRequest.Create(req.Uri.AbsoluteUri + "&" + queryString);
			else 
				request = (HttpWebRequest)WebRequest.Create(req.Uri.AbsoluteUri + "?" + queryString);
			return request;
		}

		#region WinRT Async

#if WINRT || WP8 || SL5 || WPF || WP7
		public static Task<HttpResponse> GetAsync(string url)
		{
			var task = new TaskCompletionSource<HttpResponse>();
			Get(url, result => ProcessAsyncResponse(task, result));
			return task.Task;
		}

		public static Task<HttpResponse> GetAsync(HttpGetRequest request)
		{
			var task = new TaskCompletionSource<HttpResponse>();
			Get(request, result => ProcessAsyncResponse(task, result));
			return task.Task;
		}

		public static Task<HttpResponse> PostAsync(string url)
		{
			var task = new TaskCompletionSource<HttpResponse>();
			Post(url, result => ProcessAsyncResponse(task, result));
			return task.Task;
		}

		private static void ProcessAsyncResponse(TaskCompletionSource<HttpResponse> task, HttpResponse result)
		{
			if (result.Successful)
				task.SetResult(result);
			else if (result.Canceled)
				task.SetCanceled();
			else
				task.SetException(result.Exception);
		}

		public static Task<HttpResponse> PostAsync(HttpPostRequest request)
		{
			var task = new TaskCompletionSource<HttpResponse>();
			Post(request, result =>
			{
				if (result.Successful)
					task.SetResult(result);
				else if (result.Canceled)
					task.SetCanceled();
				else
					task.SetException(result.Exception);
			});
			return task.Task;
		}
#endif

		#endregion

		#region GET

#if !WINRT
		public static HttpResponse Get(string uri, Action<HttpResponse> action, Dispatcher dispatcher)
		{
			return Get(new HttpGetRequest(uri), r => dispatcher.BeginInvoke(() => action(r)));
		}

		public static HttpResponse Get(HttpGetRequest request, Action<HttpResponse> action, Dispatcher dispatcher)
		{
			return Get(request, r => dispatcher.BeginInvoke(() => action(r)));
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
				if (!req.UseCache)
					req.Query["__dcachetime"] = DateTime.Now.Ticks.ToString(); 
				var request = CreateRequest(req);

				if (req.Credentials != null)
					request.Credentials = req.Credentials;
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

				if (req.Headers.Count > 0)
				{
					foreach (var item in req.Headers)
					{
#if WINRT
						if (item.Key != "User-Agent") // TODO fix this => cannot set UserAgent in WinRT
							request.Headers[item.Key] = item.Value;
#elif WPF
						if (item.Key != "User-Agent")
							request.Headers[item.Key] = item.Value;
						else
							request.UserAgent = item.Value; 
#else
						request.Headers[item.Key] = item.Value;
#endif
					}
				}

#if USE_GZIP
				if (req.RequestGzip)
					request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
#endif

				response.CreateTimeoutTimer(request);
				request.BeginGetResponse(r => ProcessResponse(r, request, response, action), request);
			}
			catch (Exception e)
			{
				response.Exception = e;
				if (action != null)
					action(response);
			}

			lock (pendingRequests)
				pendingRequests.Add(response);
			return response;
		}

		#endregion

		#region POST

#if !WINRT
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
			return Post(req, action, "POST");
		}
		
		public static HttpResponse Put(string uri, Action<HttpResponse> action)
		{
			return Post(new HttpPostRequest(uri), action, "PUT");
		}
		
		public static HttpResponse Put(HttpPostRequest req, Action<HttpResponse> action)
		{
			return Post(req, action, "PUT");
		}
		
		public static HttpResponse Delete(string uri, Action<HttpResponse> action)
		{
			return Post(new HttpPostRequest(uri), action, "DELETE");
		}
		
		public static HttpResponse Delete(HttpPostRequest req, Action<HttpResponse> action)
		{
			return Post(req, action, "DELETE");
		}
		
		private static HttpResponse Post(HttpPostRequest req, Action<HttpResponse> action, string method)
		{
			var response = new HttpResponse(req);
			try
			{
				var boundary = "";
				var request = CreateRequest(req);

				if (req.Credentials != null)
					request.Credentials = req.Credentials;
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

				request.Method = method;
				if (req.ContentType != null)
					request.ContentType = req.ContentType;

				if (req.Headers.Count > 0)
				{
					foreach (var item in req.Headers)
						request.Headers[item.Key] = item.Value;
				}

#if USE_GZIP
				if (req.RequestGzip)
					request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
#endif

				response.CreateTimeoutTimer(request);
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

			lock (pendingRequests) 
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

			const string dataHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
			if (request.RawData != null)
				throw new Exception("RawData not allowed if uploading files");

			foreach (var tuple in request.Data)
			{
				stream.Write(boundarybytes, 0, boundarybytes.Length);

				var formitem = string.Format(dataHeaderTemplate, tuple.Key, tuple.Value);
				var formitembytes = request.Encoding.GetBytes(formitem);
				stream.Write(formitembytes, 0, formitembytes.Length);
			}

			const string fileHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			foreach (var file in request.Files)
			{
				stream.Write(boundarybytes, 0, boundarybytes.Length);

				var header = string.Format(fileHeaderTemplate, file.Name, file.Filename, file.ContentType ?? "application/octet-stream");
				var headerbytes = request.Encoding.GetBytes(header);
				stream.Write(headerbytes, 0, headerbytes.Length);

				var fileStream = file.Stream;

#if WINRT
				if (fileStream == null)
				{
					var operation = file.File.OpenReadAsync();
					var task = operation.AsTask();
					task.RunSynchronously();
					fileStream = operation.GetResults().AsStreamForRead();
				}
#else
#if SL4 || SL5 || WP7 || WP8
				if (fileStream == null)
				{
					var isf = IsolatedStorageFile.GetUserStoreForApplication();
					fileStream = new IsolatedStorageFileStream(file.Path, FileMode.Open, FileAccess.Read, isf);
				}
#else
				if (fileStream == null)
					fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read);
#endif
#endif

				try
				{
					var buffer = new byte[1024];
					var bytesRead = 0;
					while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						stream.Write(buffer, 0, bytesRead);
						//stream.Flush();
						// TODO call progress changed event handler (calculate progress over all files) => use flush to have correct behaviour
						// TODO add progress for download
					}
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
			lock (pendingRequests)
			{
				if (pendingRequests.Contains(resp))
					pendingRequests.Remove(resp);
			}
			
			try
			{
				var response = request.EndGetResponse(asyncResult);
				resp.IsConnected = true; 
				LoadResponse(resp, response);
			}
			catch (Exception exception)
			{
				if (exception is WebException)
				{
					var response = ((WebException)exception).Response as HttpWebResponse;
					if (response != null)
					{
						resp.HttpStatusCode = response.StatusCode;
						
						try { LoadResponse(resp, response); }
						catch { }
						
						exception = new HttpStatusException(exception.Message)
						{
							Result = resp, 
							WebException = (WebException) exception
						};
					}
				}

				if (resp.ResponseStream != null)
				{
					using (resp.ResponseStream)
						resp.RawResponse = resp.ResponseStream.ReadToEnd();
				}

				resp.Exception = exception; 
				if (action != null)
					action(resp);
				return;
			}

			if (action != null)
				action(resp);
		}

		private static void LoadResponse(HttpResponse resp, WebResponse response)
		{
			using (response)
			{
				var stream = response.GetResponseStream();

#if USE_GZIP
#if WINRT || WP8
				if (response.Headers["Content-Encoding"] == "gzip")
					stream = new GZipStream(stream, CompressionMode.Decompress);
#elif SL4 || SL5 || WP7
				if (response.Headers[HttpRequestHeader.ContentEncoding] == "gzip")
					stream = new GZipStream(stream, CompressionMode.Decompress);
#else
				if (response.Headers[HttpResponseHeader.ContentEncoding] == "gzip")
					stream = new GZipStream(stream, CompressionMode.Decompress);
#endif
#endif

				if (resp.Request.ResponseAsStream)
					resp.ResponseStream = stream;
				else
				{
					using (stream)
						resp.RawResponse = stream.ReadToEnd();
				}

				if (response.Headers.AllKeys.Contains("Set-Cookie"))
				{
					var cookies = response.Headers["Set-Cookie"];
					var index = cookies.IndexOf(';');
					if (index != -1)
					{
						foreach (var c in HttpUtilityExtensions.ParseQueryString(cookies.Substring(0, index)))
							resp.Cookies.Add(new Cookie(c.Key, c.Value));
					}
				}

				foreach (var key in response.Headers.AllKeys)
				{
					var value = response.Headers[key];
					resp.Headers.Add(key, value);
				}
			}
		}

		private static string GetQueryString(Dictionary<string, string> query)
		{
			var queryString = "";
			foreach (var p in query)
				queryString += Uri.EscapeDataString(p.Key) + "=" + (p.Value == null ? "" : p.Value.EscapeUriString()) + "&";
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

#if WINRT

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

#if SL4 || SL5 || WINRT || WP8

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
