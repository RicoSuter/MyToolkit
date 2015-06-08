using System.IO;

namespace MyToolkit
{
	public class Server
	{
		public static string Root
		{
			get { return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath; }
		}

		public static string MapPath(string path)
		{
			return Path.Combine(Root, path);
		}
	}
}
