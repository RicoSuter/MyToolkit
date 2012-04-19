using Microsoft.Phone.Net.NetworkInformation;

namespace MyToolkit.Networking
{
	public static class NetworkState
	{
		public static bool IsMobileConnected
		{
			get
			{
				return NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.MobileBroadbandGsm ||
					NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.MobileBroadbandCdma;
			}
		}

		public static bool IsEthernetConnected
		{
			get
			{
				return NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet;
			}
		}

		public static bool IsWirelessConnected
		{
			get
			{
				return NetworkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211;
			}
		}

		public static bool IsConnected
		{
			get { return NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None; }
		}
	}
}