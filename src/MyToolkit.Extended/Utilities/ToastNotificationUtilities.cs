//-----------------------------------------------------------------------
// <copyright file="ToastNotificationUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace MyToolkit.Utilities
{
    /// <summary>Provides helper methods to show toast notifications. </summary>
    public static class ToastNotificationUtilities
    {
        /// <summary>Shows a toast message with the given message. </summary>
        /// <param name="message">The message to show. </param>
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

#endif