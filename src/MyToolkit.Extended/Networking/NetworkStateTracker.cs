//-----------------------------------------------------------------------
// <copyright file="NetworkStateTracker.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WINRT && !WPF

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Phone.Net.NetworkInformation;

namespace MyToolkit.Networking
{
	public class NetworkStateTracker
	{
		private static readonly Object internalLock = new Object();
		private static bool isTracking = false;

		/// <summary>
		/// Call this method in Application_Launching and Application_Activated
		/// </summary>
		public static void Start()
		{
			lock (internalLock)
			{
				if (isTracking) return;
				isTracking = true;

				isCdmaConnected = false;
				isGsmConnected = false;
				isWiFiConnected = false;

				DeviceNetworkInformation.NetworkAvailabilityChanged += OnNetworkChanged;
				NetworkState.GetNetworkInterfaceType(t => UpdateNetworkState(t, true));
			}

			ThreadPool.QueueUserWorkItem(o =>
			{
				//var types = 
				var interfaces = new NetworkInterfaceList();
				lock (internalLock)
				{
					var types = new List<NetworkInterfaceType>();
					foreach (var i in interfaces)
					{
						if (!types.Contains(i.InterfaceType))
							UpdateNetworkState(i.InterfaceType, i.InterfaceState == ConnectState.Connected);
						types.Add(i.InterfaceType);
					}
				}
			});
		}

		/// <summary>
		/// Call this method in Application_Deactivated and Application_Closing
		/// </summary>
		public static void Stop()
		{
			lock (internalLock)
			{
				if (!isTracking) return;
				isTracking = false;
				DeviceNetworkInformation.NetworkAvailabilityChanged -= OnNetworkChanged;
			}
		}

		private static void UpdateNetworkState(NetworkInterfaceType type, bool connected)
		{
			lock (internalLock)
			{
				if (type == NetworkInterfaceType.Wireless80211)
					isWiFiConnected = connected;
				else if (type == NetworkInterfaceType.MobileBroadbandCdma)
					isCdmaConnected = connected;
				else if (type == NetworkInterfaceType.MobileBroadbandGsm)
					isGsmConnected = connected;
			}
		}

		private static void OnNetworkChanged(object sender, NetworkNotificationEventArgs e)
		{
			if (e.NotificationType == NetworkNotificationType.CharacteristicUpdate)
				return;

			UpdateNetworkState(e.NetworkInterface.InterfaceType, 
				e.NotificationType == NetworkNotificationType.InterfaceConnected);
		}

		public static bool IsConnected
		{
			get { return NetworkInterface.GetIsNetworkAvailable(); }
		}

		private static bool isWiFiConnected = false;
		public static bool IsWiFiConnected
		{
			get
			{
				lock (internalLock)
				{
					if (!isTracking) throw new Exception();
					return isWiFiConnected;
				}
			}
		}

		private static bool isGsmConnected = false;
		private static bool isCdmaConnected = false;
		public static bool IsMobileConnected
		{
			get
			{
				lock (internalLock)
				{
					if (!isTracking) throw new Exception();
					return isGsmConnected || isCdmaConnected;
				}
			}
		}
	}
}

#endif