using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

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
