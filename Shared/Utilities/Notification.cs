using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using MyToolkit.Messages;
using MyToolkit.Network;
using MyToolkit.Phone;

namespace MyToolkit.Utilities.Notifications
{
	[XmlRoot("Notifications")]
	public class Notifications
	{
		[XmlElement("Notification")]
		public List<Notification> List { get; set; }
	}

	[XmlType("Message")]
	public class Message
	{
		[XmlElement(IsNullable = true)]
		public string Language { get; set; }
		[XmlElement]
		public string Title { get; set; }
		[XmlElement]
		public string Text { get; set; }
	}

	[XmlType("Notification")]
	public class Notification
	{
		[XmlElement]
		public DateTime Date { get; set; }
		[XmlElement("Message")]
		public List<Message> Messages { get; set; }

		public static void Check(string url, Action<Exception> failure = null)
		{
			Http.Get(url, r => Deployment.Current.Dispatcher.BeginInvoke(() => OnAction(r, failure)));
		}

		private static void OnAction(HttpResponse response, Action<Exception> failure)
		{
			if (response.Successful)
			{
				try
				{
					var language = CultureInfo.CurrentCulture.IsNeutralCulture
						? CultureInfo.CurrentCulture.Name
						: CultureInfo.CurrentCulture.Parent.Name;

					var date = Settings.GetSetting("MyToolkitNotificationDate", DateTime.MinValue);
					var notifications = Xml.Deserialize<Notifications>(response.Response);
					foreach (var n in notifications.List.OrderBy(n => n.Date))
					{
						if (n.Date >= date)
						{
							var message = n.Messages.SingleOrDefault(m => m.Language == language) ?? 
								n.Messages.Single(m => m.Language == null);

							Messenger.Send(new TextMessage(message.Text, message.Title));
						}
					}
					Settings.SetSetting("MyToolkitNotificationDate", DateTime.Now, true);
				}
				catch(Exception ex)
				{
					if (failure != null)
						failure(ex);
				}
			}
			else
			{
				if (failure != null)
					failure(response.Exception);
			}
		}
	}
}
