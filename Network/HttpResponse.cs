using System;
using System.Net;

namespace MyToolkit.Network
{
	public class HttpResponse
	{
		public HttpResponse(IHttpRequest request)
		{
			Request = request;
		}

		public IHttpRequest Request { get; internal set; }
		internal HttpWebRequest WebRequest { get; set; }

		public void Abort()
		{
			if (Request != null && !Successful)
				WebRequest.Abort();
		}

		private Exception exception;
		public Exception Exception
		{
			get { return exception; }
			set
			{
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
		public bool Successful { get { return !Canceled && Exception == null && Response != null; } }
		public bool HasException { get { return Exception != null; } }

		/// <summary>
		/// If Response is null and Exception is null as well the request has been canceled
		/// </summary>
		public string Response { get; internal set; }
	}
}