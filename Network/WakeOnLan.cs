using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows;

namespace MyToolkit.Network
{
	public static class WakeOnLan
	{
		public static byte[] MacAddressToBytes(string macAddress) 
		{
			var macBytes = new byte[6];
			var macAddr = Regex.Replace(macAddress, @"[^0-9A-Fa-f]", ""); 
			if (macAddr.Length != 12)
				throw new ArgumentException("macAddress"); 
			
			for (var i=0; i < macBytes.Length; i++)
			{
				var hex = new String( new [] { macAddr[i*2], macAddr[i*2+1] } );
				macBytes[i] = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber); 
			}

			return macBytes; 
		}

		public static void Wake(string macAddress, Action sentAction, Action<SocketError> failureAction)
		{
			Wake(new IPEndPoint(IPAddress.Broadcast, 7), MacAddressToBytes(macAddress), sentAction, failureAction);
		}

		public static void Wake(string host, int port, string macAddress, Action sentAction, Action<SocketError> failureAction)
		{
			Wake(new DnsEndPoint(host, port), MacAddressToBytes(macAddress), sentAction, failureAction);
		}

		public static void Wake(EndPoint endPoint, byte[] macAddress, Action sentAction, Action<SocketError> failureAction)
		{
			var packet = new List<byte>();
			for (var i = 0; i < 6; i++) // Trailer of 6 FF packets
				packet.Add(0xFF);
			for (var i = 0; i < 16; i++) // Repeat 16 times the MAC address
				packet.AddRange(macAddress);
			
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			var args = new SocketAsyncEventArgs();
			args.RemoteEndPoint = endPoint;
			args.Completed += (s, e) =>
			{
				if (e.SocketError != SocketError.Success)
				{
					Deployment.Current.Dispatcher.BeginInvoke(() => failureAction(e.SocketError));
					return;
				}

				switch (e.LastOperation)
				{
					case SocketAsyncOperation.Connect:
						e.SetBuffer(packet.ToArray(), 0, packet.Count);
						if (!socket.SendAsync(e))
							Deployment.Current.Dispatcher.BeginInvoke(sentAction);
						break;
					case SocketAsyncOperation.Send:
						Deployment.Current.Dispatcher.BeginInvoke(sentAction);
						break;
					default:
						throw new Exception("Invalid operation completed");
				}
			};
			socket.ConnectAsync(args);
		}
	}
}
