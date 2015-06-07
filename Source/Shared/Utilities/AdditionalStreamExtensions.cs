using System;
using System.IO;
using System.Net;

#if WINRT || WP8
using Windows.Storage.Streams;
#endif

namespace MyToolkit.Utilities
{
	public static class AdditionalStreamExtensions
	{
#if WP8
		public static IRandomAccessStream AsRandomAccessStream(this byte[] bytes)
		{
			return new MemoryRandomAccessStream(bytes);
		}

		public static IRandomAccessStream AsRandomAccessStream(this Stream stream)
		{
			// TODO: Allocates a lot of memory
			return new MemoryRandomAccessStream(stream.ReadToEnd());
		}
#endif
	}

#if WP8
	internal class MemoryRandomAccessStream : IRandomAccessStream
	{
		private readonly Stream internalStream;

		public MemoryRandomAccessStream(Stream stream)
		{
			internalStream = stream;
		}
		
		public MemoryRandomAccessStream(byte[] bytes)
		{
			internalStream = new MemoryStream(bytes);
		}

		public IInputStream GetInputStreamAt(ulong position)
		{
			internalStream.Position = (long)position;
			return internalStream.AsInputStream();
		}

		public IOutputStream GetOutputStreamAt(ulong position)
		{
			internalStream.Position = (long)position;
			return internalStream.AsOutputStream();
		}

		public ulong Size
		{
			get { return (ulong)internalStream.Length; }
			set { internalStream.SetLength((long)value); }
		}

		public bool CanRead
		{
			get { return true; }
		}

		public bool CanWrite
		{
			get { return true; }
		}

		public IRandomAccessStream CloneStream()
		{
			throw new NotSupportedException();
		}

		public ulong Position
		{
			get { return (ulong)internalStream.Position; }
		}

		public void Seek(ulong position)
		{
			internalStream.Seek((long)position, 0);
		}

		public void Dispose()
		{
			internalStream.Dispose();
		}

		public Windows.Foundation.IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
		{
			var inputStream = GetInputStreamAt(0);
			return inputStream.ReadAsync(buffer, count, options);
		}

		public Windows.Foundation.IAsyncOperation<bool>

		FlushAsync()
		{
			var outputStream = GetOutputStreamAt(0);
			return outputStream.FlushAsync();
		}

		public Windows.Foundation.IAsyncOperationWithProgress<uint, uint>

		WriteAsync(IBuffer buffer)
		{
			var outputStream = this.GetOutputStreamAt(0);
			return outputStream.WriteAsync(buffer);
		}
	}

#endif
}
