using System.Collections.Generic;

namespace MyToolkit.Network
{
	public class HttpPostRequest : HttpGetRequest
	{
		public HttpPostRequest(string uri) : base(uri)
		{
			Data = new Dictionary<string, string>();
			Files = new List<HttpPostFile>();
		}

		public byte[] RawData { get; set; }
		public Dictionary<string, string> Data { get; private set; }
		public List<HttpPostFile> Files { get; private set; }
	}
}