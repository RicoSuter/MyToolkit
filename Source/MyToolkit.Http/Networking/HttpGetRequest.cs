using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyToolkit.Networking
{
    /// <summary>
    /// Describes a HTTP GET request. 
    /// </summary>
    public class HttpGetRequest : HttpRequest
    {
        public HttpGetRequest(string uri)
            : this(new Uri(uri, UriKind.RelativeOrAbsolute))
        { }

        public HttpGetRequest(string uri, Dictionary<string, string> query)
            : this(new Uri(uri, UriKind.RelativeOrAbsolute), query)
        { }

        public HttpGetRequest(Uri uri)
            : this(uri, new Dictionary<string, string>())
        { }

        public HttpGetRequest(Uri uri, Dictionary<string, string> query) 
        {
            Uri = uri;
            Query = query ?? new Dictionary<string, string>();
            Headers = new Dictionary<string, string>(); 
            Cookies = new List<Cookie>();
            UseCache = true;
            Encoding = Encoding.UTF8;
            Timeout = TimeSpan.Zero;
            ResponseAsStream = false;
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip; 
        }

        public virtual Task<HttpResponse> GetAsync()
        {
            return Http.GetAsync(this);
        }

        public virtual Task<HttpResponse> GetAsync(CancellationToken token)
        {
            return Http.GetAsync(this, token);
        }
    }
}