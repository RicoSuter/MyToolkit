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
				return NetworkInformation.GetConnectionProfiles().
					Where(p => p.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.ConstrainedInternetAccess).
					Select(p => p.NetworkAdapter.NetworkAdapterId).
					OrderBy(p => p).First().ToString();
			}
		}
	}
}
