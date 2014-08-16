using System.IO;

#if WINRT
using Windows.Storage;
#endif

namespace MyToolkit.Networking
{
	public class HttpPostFile
	{
#if !WINRT
		public HttpPostFile(string name, string filename, string path)
		{
			Name = name;
			Filename = filename;
			Path = path;
			CloseStream = true; 
		}

		public HttpPostFile(string name, string path)
		{
			Name = name;
			Filename = System.IO.Path.GetFileName(path);
			Path = path;
			CloseStream = true;
		}
#else
		public HttpPostFile(string name, string filename, StorageFile file)
		{
			Name = name;
			Filename = filename;
			File = file;
			CloseStream = true;
		}

		public HttpPostFile(string name, StorageFile file)
		{
			Name = name;
			Filename = file.Name;
			File = file;
			CloseStream = true;
		}
#endif

		public HttpPostFile(string name, string filename, Stream stream, bool closeStream = true)
		{
			Name = name;
#if WINRT
			Filename = filename;
#else
			Filename = System.IO.Path.GetFileName(filename);
#endif
			Stream = stream;
			CloseStream = closeStream;
		}

		public string Name { get; private set; }
		public string Filename { get; private set; }
#if WINRT
		public StorageFile File { get; private set; }
#else
		public string Path { get; private set; } // use Path OR Stream
#endif
		public Stream Stream { get; private set; }
		public bool CloseStream { get; private set; }

		/// <summary>
		/// default: application/octet-stream
		/// </summary>
		public string ContentType { get; set; } 
	}
}