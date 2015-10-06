//-----------------------------------------------------------------------
// <copyright file="NetworkState.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WINRT && !WPF

using System;
using System.Threading;
using Microsoft.Phone.Net.NetworkInformation;

namespace MyToolkit.Networking
{
	public static class NetworkState
	{
		public static void GetNetworkInterfaceType(Action<NetworkInterfaceType> completed)
		{
			ThreadPool.QueueUserWorkItem(o =>
			{
			    var type = NetworkInterface.NetworkInterfaceType;
			    completed(type);
			});
		}

		public static void IsMobileConnected(Action<bool> completed)
		{
			GetNetworkInterfaceType(type => completed(type == NetworkInterfaceType.MobileBroadbandGsm ||
			                                          type == NetworkInterfaceType.MobileBroadbandCdma));
		}

		public static void IsEthernetConnected(Action<bool> completed)
		{
			GetNetworkInterfaceType(type => completed(type == NetworkInterfaceType.Ethernet));
		}

		public static void IsWirelessOrEthernetConnected(Action<bool> completed)
		{
			GetNetworkInterfaceType(type => completed(type == NetworkInterfaceType.Wireless80211 ||
													 type == NetworkInterfaceType.Ethernet));
		}

		public static void IsWiFiConnected(Action<bool> completed)
		{
			GetNetworkInterfaceType(type => completed(type == NetworkInterfaceType.Wireless80211));
		}

		public static void IsConnected(Action<bool> completed)
		{
			GetNetworkInterfaceType(type => completed(type != NetworkInterfaceType.None));
		}
	}
}

#endif