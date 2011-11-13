using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MyToolkit.Network
{
	public interface IHttpRequest
	{
		string Uri { get; }
		Encoding Encoding { get; }
		object Tag { get; }
	}

	public class HttpGetRequest : IHttpRequest
	{
		public HttpGetRequest(string uri)
			: this(uri, new Dictionary<string, string>())
		{ }

		public HttpGetRequest(string uri, Dictionary<string, string> query) 
		{
			Uri = uri;
			Query = query;
			Cookies = new List<Cookie>();
			UseCache = true;
			Encoding = Encoding.UTF8;

			#if USE_GZIP
			RequestGZIP = true; 
			#endif
		}

		public string Uri { get; private set; }
		public Dictionary<string, string> Query { get; private set; }
		public List<Cookie> Cookies { get; private set; }

		public bool UseCache { get; set; }
		public Encoding Encoding { get; set; }

		public object Tag { get; set; }

		#if USE_GZIP
		public bool RequestGZIP { get; set; }
		#endif
	}
}