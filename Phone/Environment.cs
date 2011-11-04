using Microsoft.Phone.Net.NetworkInformation;

namespace MyToolkit.Phone
{
	public static class PhoneEnvironment
	{
		public static bool IsWirelessConnected
		{
			get
			{
				foreach (var i in new NetworkInterfaceList())
				{
					if (i.InterfaceType == NetworkInterfaceType.Wireless80211 && i.InterfaceState == ConnectState.Connected)
						return true; 
				}
				return false; 
			}
		}
	}
}