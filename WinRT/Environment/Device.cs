using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace MyToolkit.Environment
{
	public class Machine
	{
		public static string DeviceID
		{
			get
			{
				// TODO: better, take WiFi not currently connected
				return NetworkInformation.GetInternetConnectionProfile().NetworkAdapter.NetworkAdapterId.ToString();
			}
		}
	}
}
