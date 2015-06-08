using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyToolkit.Networking
{
    /// <summary>
    /// Describes a HTTP POST request. 
    /// </summary>
    public class HttpPostRequest : HttpGetRequest
    {
        public HttpPostRequest(Uri uri)
            : base(uri)
        {
            Data = new Dictionary<string, string>();
            Files = new List<HttpPostFile>();
        }

        public HttpPostRequest(string uri) 
            : this(new Uri(uri, UriKind.RelativeOrAbsolute))
        { }

        public Task<HttpResponse> PostAsync()
        {
            return Http.PostAsync(this);
        }

        public Task<HttpResponse> PostAsync(CancellationToken token)
        {
            return Http.PostAsync(this, token);
        }

        /// <summary>
        /// Gets or sets the raw POST data (if set the Data property is ignored). 
        /// </summary>
        public byte[] RawData { get; set; }

        /// <summary>
        /// Gets or sets the POST data as key value pairs. 
        /// </summary>
        public Dictionary<string, string> Data { get; private set; }

        /// <summary>
        /// Gets or sets the files which are transmitted (as MultipartFormData). 
        /// </summary>
        public List<HttpPostFile> Files { get; private set; }
    }
}