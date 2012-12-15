using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

#if WINRT
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#elif WINPRT
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#else
using System.Net;
using System.Net.Sockets;
using System.Windows;
#endif

namespace MyToolkit.Networking
{
	public static class WakeOnLan
	{
		public static byte[] MacAddressToBytes(string macAddress) 
		{
			try
			{
				var macBytes = new byte[6];
				var macAddr = Regex.Replace(macAddress, @"[^0-9A-Fa-f]", "");
				if (macAddr.Length != 12)
					throw new ArgumentException("macAddress");

				for (var i = 0; i < macBytes.Length; i++)
				{
					var hex = new String(new[] { macAddr[i * 2], macAddr[i * 2 + 1] });
					macBytes[i] = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
				}

				return macBytes; 				
			}
			catch
			{
				throw new ArgumentException("macAddress");
			}
		}

#if WINRT || WINPRT
		public static Task WakeAsync(string macAddress)
		{
			return WakeAsync(new HostName("255.255.255.255"), 7, MacAddressToBytes(macAddress));
		}

		public static Task WakeAsync(string host, int port, string macAddress)
		{
			return WakeAsync(new HostName(host), 7, MacAddressToBytes(macAddress));
		}

		public static async Task WakeAsync(HostName endPoint, int port, byte[] macAddress)
		{
			var packet = new List<byte>();
			for (var i = 0; i < 6; i++) // Trailer of 6 FF packets
				packet.Add(0xFF);
			for (var i = 0; i < 16; i++) // Repeat 16 times the MAC address
				packet.AddRange(macAddress);

			using (var socket = new DatagramSocket())
			{
				await socket.ConnectAsync(endPoint, port.ToString());
				var stream = socket.OutputStream;
				using (var writer = new DataWriter(stream))
				{
					writer.WriteBytes(packet.ToArray());
					await writer.StoreAsync();
				}				
			}
		}
#endif
#if !WINRT
		public static void Wake(string macAddress, Action sentAction, Action<System.Net.Sockets.SocketError> failureAction)
		{
			Wake(new IPEndPoint(IPAddress.Broadcast, 7), MacAddressToBytes(macAddress), sentAction, failureAction);
		}

		public static void Wake(string host, int port, string macAddress, Action sentAction, Action<System.Net.Sockets.SocketError> failureAction)
		{
			Wake(new DnsEndPoint(host, port), MacAddressToBytes(macAddress), sentAction, failureAction);
		}

		public static void Wake(EndPoint endPoint, byte[] macAddress, Action sentAction, Action<System.Net.Sockets.SocketError> failureAction)
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
				if (e.SocketError != System.Net.Sockets.SocketError.Success)
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
#endif
	}
}
