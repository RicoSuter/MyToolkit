using System.IO;

namespace MyToolkit.Networking
{
	public class HttpPostFile
	{
#if !METRO
		public HttpPostFile(string name, string filename, string path)
		{
			Name = name;
			Filename = System.IO.Path.GetFileName(filename);
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
#endif

		public HttpPostFile(string name, string filename, Stream stream, bool closeStream = true)
		{
			Name = name;
#if METRO
				Filename = filename;
#else
			Filename = System.IO.Path.GetFileName(filename);
#endif
			Stream = stream;
			CloseStream = closeStream;
		}

		public string Name { get; private set; }
		public string Filename { get; private set; }
		public string Path { get; private set; } // use Path OR Stream
		public Stream Stream { get; private set; }
		public bool CloseStream { get; private set; }

		public string ContentType { get; set; } // default: application/octet-stream
	}
}