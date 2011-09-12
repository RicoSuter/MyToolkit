using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MyToolkit.Network;

namespace MyToolkit.Utilities
{
	public static class Image
	{
		public static void Preload(string url)
		{
			Http.Get(new GetRequest(url) { UseCache = false }, null);
		}
	}
}
