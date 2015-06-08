using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MyToolkit.Networking
{
    /// <summary>
    /// Describes a HTTP request. 
    /// </summary>
    public abstract class HttpRequest
    {
        /// <summary>
        /// Gets or sets the uri to retrieve. 
        /// </summary>
        public Uri Uri { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum allowed duration till FIRST response from server; 
        /// Download of message will never be cancelled. 
        /// If TimeSpan.Zero then use default timeout (recommended)
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed duration till FIRST response from server; 
        /// Download of message will never be cancelled. 
        /// If TimeSpan.Zero then use default timeout (recommended)
        /// </summary>
        [Obsolete("Use Timeout property instead. 7/1/2014")]
        public int ConnectionTimeout
        {
            get { return (int)Timeout.TotalSeconds; }
            set { Timeout = value != 0 ? TimeSpan.FromSeconds(value) : TimeSpan.Zero; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the response should be returned as string (RawResponse is null). 
        /// </summary>
        public bool ResponseAsStream { get; set; }

        /// <summary>
        /// Gets or sets the query for the HTTP request (part after the ? of the URI). 
        /// </summary>
        public Dictionary<string, string> Query { get; protected set; }

        /// <summary>
        /// Gets or sets the HTTP cookies. 
        /// </summary>
        public List<Cookie> Cookies { get; protected set; }

        /// <summary>
        /// Gets or sets the HTTP headers. 
        /// </summary>
        public Dictionary<string, string> Headers { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether caching is enabled (default is true). 
        /// </summary>
        public bool UseCache { get; set; }

        /// <summary>
        /// Gets or sets the encoding which is used decode the HTTP response to a string (default UTF-8). 
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets a user tag. 
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets or sets the credentials for the HTTP request. 
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// Gets or sets the accept type (the 'Accept' HTTP header). 
        /// </summary>
        public string Accept
        {
            get { return Headers.ContainsKey("Accept") ? Headers["Accept"] : null; }
            set { Headers["Accept"] = value; }
        }

        /// <summary>
        /// Gets or sets the content type (the 'Content-Type' HTTP header). 
        /// </summary>
        public string ContentType
        {
            get { return Headers.ContainsKey("Content-Type") ? Headers["Content-Type"] : null; }
            set { Headers["Content-Type"] = value; }
        }

        /// <summary>
        /// Gets or sets the user agent (the 'User-Agent' HTTP header). 
        /// </summary>
        public string UserAgent
        {
            get { return Headers.ContainsKey("User-Agent") ? Headers["User-Agent"] : null; }
            set { Headers["User-Agent"] = value; }
        }

        /// <summary>
        /// Sets the Authorization HTTP header to a basic authentication. 
        /// </summary>
        /// <param name="username">The username. </param>
        /// <param name="password">The password. </param>
        public void SetBasicAuthenticationHeader(String username, String password)
        {
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            Headers["Authorization"] = "Basic " + auth;
        }

        /// <summary>
        /// Sets or gets the automatic decompression methods (GZIP or DEFLATE compression enabled by default). 
        /// </summary>
        public DecompressionMethods AutomaticDecompression { get; set; } 
    }
}