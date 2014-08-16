using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace MyToolkit.Networking
{
	public class HttpResponse
	{
		internal HttpClient HttpClient { get; set; }
		internal HttpResponse()
		{
			Cookies = new List<Cookie>();
			Headers = new Dictionary<string, string>();
		}

		public HttpResponse(HttpRequest request) : this()
		{
			Request = request;
		}

		/// <summary>
		/// Gets the original request of this response
		/// </summary>
		public HttpRequest Request { get; internal set; }

		public void Abort()
		{
			if (Request != null)
			{
				try
				{
					HttpClient.CancelPendingRequests();
				}
				catch  { }
			}
		}

		private Exception _exception;
		/// <summary>
		/// Gets an eventual exception after the request has processed
		/// </summary>
		public Exception Exception
		{
			get { return _exception; }
			set
			{
				if (_exception is TimeoutException)
					return; // already set

				if (value is OperationCanceledException ||
					(value is WebException && ((WebException)value).Status == WebExceptionStatus.RequestCanceled))
				{
					_exception = null;
					Canceled = true;
				}
				else
				{
					_exception = value;
					Canceled = false;
				}
			}
		}

		/// <summary>
		/// Gets a value that indicates whether the request has been processed (can be used for polling (not recommended))
		/// </summary>
		public bool Processed { get { return Canceled || Successful || HasException; } }

		/// <summary>
		/// Gets a value that indicates whether the request has been cancelled
		/// </summary>
		public bool Canceled { get; internal set; }

		/// <summary>
		/// Gets a value that indicates whether the request has been successfully processed
		/// </summary>
		public bool Successful { get { return !Canceled && Exception == null && (Response != null || ResponseStream != null); } }
		
		/// <summary>
		/// Gets a value that indicates whether the request has had an exception
		/// </summary>
		public bool HasException { get { return Exception != null; } }

		/// <summary>
		/// Gets a the response as string decoded using the Encoding from the request 
		/// (null if the request has had an exception or has been caneled)
		/// </summary>
		public string Response { get { return RawResponse == null ? null : Request.Encoding.GetString(RawResponse, 0, RawResponse.Length); } }
		
		/// <summary>
		/// Gets a the response as byte array
		/// </summary>
		public byte[] RawResponse { get; internal set; }

		/// <summary>
		/// Gets a the response as stream (only if ResponseAsStream has been set on the request) 
		/// </summary>
		public Stream ResponseStream { get; internal set; }

		/// <summary>
		/// Gets the received HTTP cookies
		/// </summary>
		public List<Cookie> Cookies { get; private set; }

		/// <summary>
		/// Gets the received HTTP headers
		/// </summary>
		public Dictionary<string, string> Headers { get; private set; }

		private HttpStatusCode _code = HttpStatusCode.OK;

		/// <summary>
		/// Gets the HTTP status code
		/// </summary>
		public HttpStatusCode HttpStatusCode
		{
			get { return _code; }
			internal set { _code = value; }
		}

		/// <summary>
		/// Creates a canceled HttpRespone object
		/// </summary>
		/// <returns></returns>
		public static HttpResponse CreateCanceled()
		{
			return new HttpResponse { Canceled = true };
		}

		/// <summary>
		/// Creates a HttpResponse with an exceptional state
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static HttpResponse CreateException(Exception exception)
		{
			return new HttpResponse { Exception = exception };
		}
	}
}