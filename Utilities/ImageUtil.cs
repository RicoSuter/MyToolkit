using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyToolkit.Network;

namespace MyToolkit.Utilities
{
	public static class ImageUtil
	{
		private static List<WebClient> list; 
		public static void Preload(string url)
		{
			if (list == null)
				list = new List<WebClient>();

			var wc = new WebClient();
			list.Add(wc);
			wc.DownloadStringAsync(new Uri(url));
			wc.DownloadStringCompleted += delegate
			{
			    list.Remove(wc); 
			};
		}
	}
}
