using Microsoft.Phone.Net.NetworkInformation;

namespace MyToolkit.Networking
{
	public static class NetworkState
	{
		public static bool IsMobileConnected
		{
			get { return DeviceNetworkInformation.IsCellularDataEnabled; }
		}

		public static bool IsWirelessConnected
		{
			get { return DeviceNetworkInformation.IsWiFiEnabled; }
		}

		public static bool IsConnected
		{
			get { return DeviceNetworkInformation.IsNetworkAvailable; }
		}
	}
}