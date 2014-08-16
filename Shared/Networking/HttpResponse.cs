using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

#if WINRT
using System.Threading.Tasks;
#endif 

namespace MyToolkit.Networking
{
	public class HttpResponse
	{
		internal HttpResponse()
		{
			Cookies = new List<Cookie>();
			Headers = new Dictionary<string, string>();
		}

		public HttpResponse(IHttpRequest request) : this()
		{
			Request = request;
		}

		public static HttpResponse CreateCancelled()
		{
			return new HttpResponse { Canceled = true };
		}

		public static HttpResponse CreateException(Exception exception)
		{
			return new HttpResponse { Exception = exception };
		}

		public IHttpRequest Request { get; internal set; }
		internal HttpWebRequest WebRequest { get; set; }

		public bool IsConnected { get; internal set; }

		public bool IsPending
		{
			get { return !HasException && !Canceled && !Successful; } 
		}

		public void Abort()
		{
			if (Request != null && !Successful)
			{
				try
				{
					WebRequest.Abort();
				}
				catch  { }
			}
		}

		private Exception exception;
		public Exception Exception
		{
			get { return exception; }
			set
			{
				if (exception is TimeoutException)
					return; // already set

				if (value is WebException && ((WebException)value).Status == WebExceptionStatus.RequestCanceled)
				{
					exception = null;
					Canceled = true;
				}
				else
				{
					exception = value;
					Canceled = false;
				}
			}
		}

		/// <summary>
		/// Can be used for polling (not recommended)
		/// </summary>
		public bool Processed { get { return Canceled || Successful || HasException; } }

		public bool Canceled { get; private set; }
		public bool Successful { get { return !Canceled && Exception == null && (Response != null || ResponseStream != null); } }
		public bool HasException { get { return Exception != null; } }

		/// <summary>
		/// If Response is null and Exception is null as well the request has been canceled
		/// </summary>
		public string Response { get { return RawResponse == null ? null : Request.Encoding.GetString(RawResponse, 0, RawResponse.Length); } }
		public byte[] RawResponse { get; internal set; }
		public Stream ResponseStream { get; internal set; }

		public List<Cookie> Cookies { get; private set; }
		public Dictionary<string, string> Headers { get; private set; }

		private HttpStatusCode code = HttpStatusCode.OK;
		public HttpStatusCode HttpStatusCode
		{
			get { return code; }
			internal set { code = value; }
		}

#if WINRT
        internal void CreateTimeoutTimer(HttpWebRequest request)
        {
			if (Request.ConnectionTimeout > 0)
			{
				request.ContinueTimeout = Request.ConnectionTimeout;
				//await Task.Delay(Request.ConnectionTimeout * 1000);
				
				//if (IsPending && !IsConnected)
				//{
				//	Exception = new TimeoutException("The connection timed out.");
				//	Abort();
				//}
			}
        }
#else
		private Timer timer; 
		internal void CreateTimeoutTimer(HttpWebRequest request)
		{
			if (Request.ConnectionTimeout > 0)
			{
				timer = new Timer(s =>
				{
					timer.Dispose();
					if (IsPending && !IsConnected)
					{
						Exception = new TimeoutException("The connection timed out.");
						Abort();
					}
				}, null, Request.ConnectionTimeout * 1000, Timeout.Infinite);
			}
		}
#endif
	}
}