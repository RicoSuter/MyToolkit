using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using MyToolkit.Utilities;

namespace MyToolkit.Notifications
{
	public class PhonePushNotificationService
	{
		public bool SendRawNotification(string url, byte[] message, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			return SendNotification(url, message, PushNotificationType.Raw, priority);
		}

		public bool SendRawNotification(string url, string message, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			return SendNotification(url, message, PushNotificationType.Raw, priority);
		}

		public bool SendToastNotification(string url, string line1, string line2, PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			var msg =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<wp:Notification xmlns:wp=\"WPNotification\">" +
				   "<wp:Toast>" +
				   "<wp:Text1>" + Xml.XmlEscape(line1) + "</ltwp:Text1>" +
				   "<wp:Text2>" + Xml.XmlEscape(line2) + "</wp:Text2>" +
				   "</ltwp:Toast>" +
				"</wp:Notification>";

			return SendNotification(url, msg, PushNotificationType.Toast, priority);
		}

		public void SendMainTileUpdate(
			string url, 
			string title, int count, string image, 
			string backTitle, string backContent, string backImage,
			PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			SendTileUpdate(url, null, title, count, image, backTitle, backContent, backImage, priority);
		}

		public bool SendMainTileCountUpdate(
			string url, int count, 
			PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			var msg = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
					  "<wp:Notification xmlns:wp=\"WPNotification\"><wp:Tile>";
			if (count >= 0)
				msg += "<wp:Count>" + count + "</wp:Count>";
			else
				msg += "<wp:Count Action=\"Clear\"></wp:Count>";
			msg += "</wp:Tile></wp:Notification>";

			return SendNotification(url, msg, PushNotificationType.Tile, priority);
		}

		public void SendTileUpdate(
			string url, 
			string navigationUri, 
			string title, int count, string image, 
			string backTitle, string backContent, string backImage,
			PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			var msg = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
			          "<wp:Notification xmlns:wp=\"WPNotification\">";

			if (string.IsNullOrEmpty(navigationUri))
				msg += "<wp:Tile>";
			else
				msg += "<wp:Tile ID=\"" + navigationUri + "\">";

			// front
			if (title != null)
				msg += "<wp:Title>" + Xml.XmlEscape(title) + "</wp:Title>";
			else
				msg += "<wp:Title Action=\"Clear\"></wp:Title>";

			if (image != null)
				msg += "<wp:BackgroundImage>" + Xml.XmlEscape(image) + "</wp:BackgroundImage>";
			else
				msg += "<wp:BackgroundImage Action=\"Clear\"></wp:BackgroundImage>";

			if (count >= 0)
				msg += "<wp:Count>" + count + "</wp:Count>";
			else
				msg += "<wp:Count Action=\"Clear\"></wp:Count>";

			// back
			if (backTitle != null)
				msg += "<wp:BackTitle>" + Xml.XmlEscape(backTitle) + "</wp:BackTitle>";
			else
				msg += "<wp:BackTitle Action=\"Clear\"></wp:BackTitle>";
			
			if (backContent != null)
				msg += "<wp:BackContent>" + Xml.XmlEscape(backContent) + "</wp:BackContent>";
			else
				msg += "<wp:BackContent Action=\"Clear\"></wp:BackContent>";

			if (backImage != null)
				msg += "<wp:BackBackgroundImage>" + Xml.XmlEscape(backImage) + "</wp:BackBackgroundImage>";
			else
				msg += "<wp:BackBackgroundImage Action=\"Clear\"></wp:BackBackgroundImage>";

			msg += "</wp:Tile></wp:Notification>";

			SendNotification(url, msg, PushNotificationType.Tile, priority);
		}

		public bool SendNotification(string url, string text, PushNotificationType target, 
			PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
			return SendNotification(url, Encoding.UTF8.GetBytes(text), target, priority);
		}

		public bool SendNotification(string url, byte[] message, PushNotificationType target, 
			PushNotificationPriority priority = PushNotificationPriority.Regular)
		{
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

				return subscriptionStatus != null && subscriptionStatus.ToLower() == "active";
			}
		}

		private string GetClassNumber(PushNotificationType target, PushNotificationPriority priority = PushNotificationPriority.Regular)
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
