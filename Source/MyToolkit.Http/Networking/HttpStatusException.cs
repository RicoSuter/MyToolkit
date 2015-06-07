using System;
using System.Net;

namespace MyToolkit.Networking
{
    /// <summary>
    /// An exception which occures when the HTTP status code != 200. 
    /// </summary>
    public class HttpStatusException : Exception
    {
        public HttpStatusException() { }

        public HttpStatusException(string message) : base("HTTP error '" + message + "'") { }

        /// <summary>
        /// Gets or sets the result as HttpResponse object. 
        /// </summary>
        public HttpResponse Result { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code. 
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}