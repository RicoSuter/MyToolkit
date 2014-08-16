using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using MyToolkit.Command;
using MyToolkit.Multimedia;
using MyToolkit.Networking;
using MyToolkit.Notifications;
using MyToolkit.Utilities;
using MyToolkit.WorkflowEngine.Activities;

namespace SampleWpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void OnSendPushNotification(object sender, RoutedEventArgs e)
        {
            // TODO: Add your credentials here
            var serverSid = "???";
            var serverSecret = "???"; 

            var uri = ChannelUriTextBox.Text;
            var service = new PushNotificationService(serverSid, serverSecret); 

            var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
			    <toast>
				    <visual>
				    <binding template=""ToastText01"">
					    <text id=""1"">Hello from the SampleWpfApplication application. </text>
				    </binding>
				    </visual>
			    </toast>";

            var status = service.SendToastNotification(uri, xml);

            Debug.WriteLine(status);
        }

        #region Library tests

        private async void Initialize()
        {
            //// Check whether all libraries can be used. 

            var command = new RelayCommand(() => { });
            Debug.WriteLine(command.CanExecute);

            var color = ColorUtility.ToHex(Colors.Green);
            Debug.WriteLine(color);

            var uri = await YouTube.GetVideoUriAsync("pYVQxKZ6-6Y", YouTubeQuality.Quality720P);
            Debug.WriteLine(uri.Uri);

            var response = await Http.GetAsync("http://www.google.ch");
            Debug.WriteLine(response.Response);
        }

        #endregion
    }
}
