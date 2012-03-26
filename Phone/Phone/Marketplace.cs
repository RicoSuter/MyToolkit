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
using Microsoft.Phone.Marketplace;

namespace MyToolkit.Phone
{
	public static class Marketplace
	{
		public static bool IsTrial
		{
			get
			{
				#if DEBUG
				return true;
				#else
				var license = new LicenseInformation();
				return license.IsTrial();
				#endif
			}
		}
	}
}
