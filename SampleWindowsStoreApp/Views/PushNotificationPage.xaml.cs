using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using MyToolkit.Storage;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class PushNotificationPage
    {
        // See http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868221.aspx

        // See sending of push notifications in the SampleWpfApplication application. 

        private const string PushNotificationChannelSettingKey = "PushNotificationChannel";

        public PushNotificationPage()
        {
            InitializeComponent();
            ChannelUriTextBox.Text = ApplicationSettings.GetSetting<string>(PushNotificationChannelSettingKey, null);
        }

        public static async void OnLoadApplication()
        {
            // TODO: Implement this directly in App.xaml.cs

            try
            {
                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                if (ApplicationSettings.GetSetting<string>(PushNotificationChannelSettingKey, null) != channel.Uri)
                {
                    ApplicationSettings.SetSetting(PushNotificationChannelSettingKey, channel.Uri);

                    // TODO: Send channel.URI to your server
                }
            }
            catch (Exception exception)
            {
                // TODO: Handle exception
                Debugger.Break();
            }
        }
    }
}
