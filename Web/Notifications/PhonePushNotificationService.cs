using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MyToolkit.Utilities;

namespace MyToolkit.Notifications
{
	public class PhonePushNotificationService
	{
		public void SendToastNotification(string url, string line1, string line2, PushNotificationClass nClass = PushNotificationClass.Regular)
		{
			string msg =
				"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
				"<wp:Notification xmlns:wp=\"WPNotification\">" +
				   "<wp:Toast>" +
				   "<wp:Text1>" + Xml.XmlEscape(line1) + "</ltwp:Text1>" +
				   "<wp:Text2>" + Xml.XmlEscape(line2) + "</wp:Text2>" +
				   "</ltwp:Toast>" +
				"</wp:Notification>";

			SendNotification(url, msg, PushNotificationType.Toast);
		}

		public void SendNotification(string url, string xml, PushNotificationType target, PushNotificationClass nClass = PushNotificationClass.Regular)
		{
			byte[] msgBytes = new UTF8Encoding().GetBytes(xml);

			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = WebRequestMethods.Http.Post;
			request.ContentType = "text/xml";
			request.ContentLength = xml.Length;
			request.Headers["X-MessageID"] = Guid.NewGuid().ToString();
			request.Headers["X-NotificationClass"] = GetClassNumber(target, nClass);

			if (target != PushNotificationType.Raw)
				request.Headers["X-WindowsPhone-Target"] = target == PushNotificationType.Toast ? "toast" : "token";

			Stream requestStream = request.GetRequestStream();
			requestStream.Write(msgBytes, 0, msgBytes.Length);
			requestStream.Close();
		}

		private static string GetClassNumber(PushNotificationType target, PushNotificationClass nClass = PushNotificationClass.Regular)
		{
			if (target == PushNotificationType.Tile)
			{
				switch (nClass)
				{
					case PushNotificationClass.Priority: return "1";
					case PushNotificationClass.Realtime: return "11";
					case PushNotificationClass.Regular: return "21";
				}
			}
			else if (target == PushNotificationType.Toast)
			{
				switch (nClass)
				{
					case PushNotificationClass.Priority: return "2";
					case PushNotificationClass.Realtime: return "12";
					case PushNotificationClass.Regular: return "22";
				}
			}
			else if (target == PushNotificationType.Raw)
			{
				switch (nClass)
				{
					case PushNotificationClass.Priority: return "3";
					case PushNotificationClass.Realtime: return "13";
					case PushNotificationClass.Regular: return "23";
				}
			}
			throw new NotImplementedException();
		}
	}
}
