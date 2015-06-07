using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Phone.Notification;

namespace MyToolkit.Notifications
{
	public static class PushNotifications
	{
		public static Task<Tuple<HttpNotificationChannel, bool>> RegisterAsync(string channelName)
		{
			var task = new TaskCompletionSource<Tuple<HttpNotificationChannel, bool>>();
			Register(channelName, (channel, created) => task.SetResult(new Tuple<HttpNotificationChannel, bool>(channel, created)));
			return task.Task;
		}

		public static HttpNotificationChannel Register(string channelName, 
			Action<HttpNotificationChannel, bool> completed)
		{
			return Register(channelName, null, true, true, null, completed);
		}

		public static Task<Tuple<HttpNotificationChannel, bool>> RegisterAsync(string channelName,
			Collection<Uri> baseUris, bool bindToShellTile, bool bindToShellToast,
			EventHandler<NotificationChannelErrorEventArgs> errorHandler)
		{
			var task = new TaskCompletionSource<Tuple<HttpNotificationChannel, bool>>();
			Register(channelName, baseUris, bindToShellTile, bindToShellToast, errorHandler, 
				(channel, created) => task.SetResult(new Tuple<HttpNotificationChannel, bool>(channel, created)));
			return task.Task;
		}

		public static HttpNotificationChannel Register(string channelName, 
			Collection<Uri> baseUris, bool bindToShellTile, bool bindToShellToast, 
			EventHandler<NotificationChannelErrorEventArgs> errorHandler, 
			Action<HttpNotificationChannel, bool> completed)
		{
			try
			{
				var channel = HttpNotificationChannel.Find(channelName);
				if (channel == null)
				{
					channel = new HttpNotificationChannel(channelName);
					channel.ChannelUriUpdated += (s, e) =>
					{
						if (!channel.IsShellTileBound && bindToShellTile)
						{
							if (baseUris != null)
								channel.BindToShellTile(baseUris);
							else
								channel.BindToShellTile();
						}

						if (!channel.IsShellToastBound && bindToShellToast)
							channel.BindToShellToast();

						completed(channel, true);
					};
					channel.ErrorOccurred += (sender, args) => completed(null, false);

					if (errorHandler != null)
						channel.ErrorOccurred += errorHandler;

					channel.Open();
				}
				else
				{
					if (errorHandler != null)
					{
						channel.ErrorOccurred -= errorHandler;
						channel.ErrorOccurred += errorHandler;
					}

					completed(channel, false);
				}
				return channel;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached)
					Debugger.Break();

				completed(null, false);
				return null; 
			}
		}
	}
}
