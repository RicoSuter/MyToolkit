using System;
using System.Collections.Generic;

namespace MyToolkit.Networking
{
	public class HttpPostRequest : HttpGetRequest
	{
		public HttpPostRequest(Uri uri)
			: base(uri)
		{
			Data = new Dictionary<string, string>();
			Files = new List<HttpPostFile>();
		}

		public HttpPostRequest(string uri) : this(new Uri(uri, UriKind.RelativeOrAbsolute))
		{ }

		public byte[] RawData { get; set; }
		public Dictionary<string, string> Data { get; private set; }
		public List<HttpPostFile> Files { get; private set; }
	}
}