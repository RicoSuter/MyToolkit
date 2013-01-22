using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace MyToolkit.Utilities
{
	public static class ToastNotificationHelper
	{
		public static void ShowMessage(string message)
		{
			var xml = string.Format("<toast><visual version='1'>" +
				"<binding template='ToastText01'><text id='1'>{0}</text></binding>" +
				"</visual></toast>", message);
	
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);

			var toast = new ToastNotification(xmlDoc);
			ToastNotificationManager.CreateToastNotifier().Show(toast);
		}
	}
}
