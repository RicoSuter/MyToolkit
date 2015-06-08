using System.IO;

namespace MyToolkit.Networking
{
    /// <summary>
    /// Describes a HTTP POST file. 
    /// </summary>
    public class HttpPostFile
    {
        public HttpPostFile(string name, string filename, Stream stream, bool closeStream = true)
        {
            Name = name;
            Filename = filename;
            Stream = stream;
            CloseStream = closeStream;
        }

        /// <summary>
        /// Gets or sets the name of the HTTP POST file. 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the filename of the HTTP POST file. 
        /// </summary>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets or sets the content of the file as stream. 
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the stream should be automatically closed after reading its content (default is true). 
        /// </summary>
        public bool CloseStream { get; private set; }

        /// <summary>
        /// Gets or sets the file's content type (the default is 'application/octet-stream'). 
        /// </summary>
        public string ContentType { get; set; } 
    }
}