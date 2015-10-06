//-----------------------------------------------------------------------
// <copyright file="OutputProcessorActionFilterAttribute.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using System.Web;

namespace MyToolkit.Filters
{
    /// <summary>Aggreges the wrapped stream's data and post-processes when all data has been written.</summary>
    public class OutputProcessorStream : Stream
    {
        private readonly StringBuilder _data = new StringBuilder();

        private readonly Stream _stream;
        private readonly Func<string, string> _processor;

        private readonly Encoding _inputEncoding;
        private readonly Encoding _outputEncoding;

        /// <summary>Initializes a new instance of the <see cref="OutputProcessorStream"/> class.</summary>
        /// <param name="response">The response.</param>
        /// <param name="processor">The processor.</param>
        public OutputProcessorStream(HttpResponseBase response, Func<string, string> processor)
            : this(response.Filter, response.ContentEncoding, response.ContentEncoding, processor)
        {
        }

        internal OutputProcessorStream(Stream stream, Encoding inputEncoding, Encoding outputEncoding, Func<string, string> processor)
        {
            _stream = stream;
            _processor = processor;
            _inputEncoding = inputEncoding;
            _outputEncoding = outputEncoding;
        }

        /// <summary>When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.</summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _data.Append(_inputEncoding.GetString(buffer, offset, count));
        }

        /// <exception cref="IOException">An I/O error has occurred.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public override void Close()
        {
            using (var writer = new StreamWriter(_stream, _outputEncoding))
            {
                writer.Write(_processor(_data.ToString()));
                writer.Flush();
            }
            _data.Clear();
        }

        /// <summary>When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.</summary>
        public override void Flush()
        {
        }

        /// <summary>When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.</summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        /// <summary>When overridden in a derived class, sets the position within the current stream.</summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>The new position within the current stream.</returns>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        /// <summary>When overridden in a derived class, sets the length of the current stream.</summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        /// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports reading.</summary>
        public override bool CanRead { get { return true; } }

        /// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports seeking.</summary>
        public override bool CanSeek { get { return true; } }

        /// <summary>When overridden in a derived class, gets a value indicating whether the current stream supports writing.</summary>
        public override bool CanWrite { get { return true; } }

        /// <summary>When overridden in a derived class, gets the length in bytes of the stream.</summary>
        public override long Length { get { return 0; } }

        /// <summary>When overridden in a derived class, gets or sets the position within the current stream.</summary>
        public override long Position { get; set; }
    }
}