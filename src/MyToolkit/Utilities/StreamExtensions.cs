//-----------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MyToolkit.Utilities
{
#if WPF40
    public interface IProgress<T>
    {
        void Report(T value);
    }
#endif

    /// <summary>Provides methods to handle streams. </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads all available bytes from the stream. 
        /// </summary>
        /// <param name="stream">The stream to read from. </param>
        /// <returns>The read byte array. </returns>
        public static byte[] ReadToEnd(this Stream stream)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        /// <summary>Asynchronously reads all available bytes from the stream. </summary>
        /// <param name="stream">The stream to read from. </param>
        /// <returns>The read byte array. </returns>
        public static Task<byte[]> ReadToEndAsync(this Stream stream)
        {
            return stream.ReadToEndAsync(CancellationToken.None);
        }

        /// <summary>Asynchronously reads all available bytes from the stream. </summary>
        /// <param name="stream">The stream to read from. </param>
        /// <param name="token">The cancellation token. </param>
        /// <param name="progress">The progress. </param>
        /// <returns>The read byte array. </returns>
        public static Task<byte[]> ReadToEndAsync(this Stream stream, CancellationToken token, IProgress<long> progress = null)
        {
            var source = new TaskCompletionSource<byte[]>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var buffer = new byte[16 * 1024];
                    using (var ms = new MemoryStream())
                    {
                        int read;
                        long totalRead = 0;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            token.ThrowIfCancellationRequested();
                            ms.Write(buffer, 0, read);
                            totalRead += read;
                            if (progress != null)
                                progress.Report(totalRead);
                        }
                        source.SetResult(ms.ToArray());
                    }
                }
                catch (Exception ex)
                {
                    source.SetException(ex);
                }
            }, token);
            return source.Task;
        }

        /// <summary>Converts a string to a memory stream. </summary>
        /// <param name="str">The string to convert. </param>
        /// <returns>The converted string. </returns>
        public static Stream ToStream(this string str)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
