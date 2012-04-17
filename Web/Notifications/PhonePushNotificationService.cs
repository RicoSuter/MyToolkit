using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MyToolkit.Utilities;

namespace MyToolkit.Notifications
{
	public static class PhonePushNotificationService
	{
		public static void SendRawNotification(string url, string message, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			SendNotification(url, message, PushNotificationType.Raw, priority);
		}

		public static void SendToastNotification(string url, string line1, string line2, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			var msg =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<wp:Notification xmlns:wp=\"WPNotification\">" +
				   "<wp:Toast>" +
				   "<wp:Text1>" + Xml.XmlEscape(line1) + "</ltwp:Text1>" +
				   "<wp:Text2>" + Xml.XmlEscape(line2) + "</wp:Text2>" +
				   "</ltwp:Toast>" +
				"</wp:Notification>";

			SendNotification(url, msg, PushNotificationType.Toast, priority);
		}

		public static void SendTileUpdate(string url, string title, int count, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			SendTileUpdate(url, title, count, null, priority);
		}

		public static void SendTileUpdate(string url, string title, int count, string backgroundImage, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			var msg = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<wp:Notification xmlns:wp=\"WPNotification\">" +
				"<wp:Tile>";

			if (backgroundImage != null)
				msg += "<wp:BackgroundImage>" + Xml.XmlEscape(backgroundImage) + "</wp:BackgroundImage>";

			msg += "<wp:Count>" + count + "</wp:Count>" +
				"<wp:Title>" + Xml.XmlEscape(title) + "</wp:Title>" +
				"</wp:Tile> " +
				"</wp:Notification>";

			SendNotification(url, msg, PushNotificationType.Tile, priority);
		}

		public static void SendNotification(string url, string xml, PushNotificationType target, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			var message = new UTF8Encoding().GetBytes(xml);

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = WebRequestMethods.Http.Post;

			request.ContentType = "text/xml";
			request.ContentLength = message.Length;

			request.Headers["X-MessageID"] = Guid.NewGuid().ToString();
			request.Headers["X-NotificationClass"] = GetClassNumber(target, priority);

			if (target != PushNotificationType.Raw)
				request.Headers["X-WindowsPhone-Target"] = target == PushNotificationType.Toast ? "toast" : "token";

			using (var requestStream = request.GetRequestStream())
				requestStream.Write(message, 0, message.Length);

			using (var response = (HttpWebResponse)request.GetResponse())
			{
				var notificationStatus = response.Headers["X-NotificationStatus"];
				var subscriptionStatus = response.Headers["X-SubscriptionStatus"];
				var deviceConnectionStatus = response.Headers["X-DeviceConnectionStatus"];

				Debug.WriteLine(string.Format("Device Connection Status: {0}", deviceConnectionStatus));
				Debug.WriteLine(string.Format("Notification Status: {0}", notificationStatus));
				Debug.WriteLine(string.Format("Subscription Status: {0}", subscriptionStatus));
			}
		}

		private static string GetClassNumber(PushNotificationType target, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			if (target == PushNotificationType.Tile)
			{
				switch (priority)
				{
					case PushNotificationPriority.Realtime: return "1";
					case PushNotificationPriority.Priority: return "11";
					case PushNotificationPriority.Regular: return "21";
				}
			}
			else if (target == PushNotificationType.Toast)
			{
				switch (priority)
				{
					case PushNotificationPriority.Realtime: return "2";
					case PushNotificationPriority.Priority: return "12";
					case PushNotificationPriority.Regular: return "22";
				}
			}
			else if (target == PushNotificationType.Raw)
			{
				switch (priority)
				{
					case PushNotificationPriority.Realtime: return "3";
					case PushNotificationPriority.Priority: return "13";
					case PushNotificationPriority.Regular: return "23";
				}
			}
			throw new NotImplementedException();
		}
	}
}
