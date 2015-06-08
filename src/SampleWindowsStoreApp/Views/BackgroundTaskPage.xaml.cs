using System;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class BackgroundTaskPage
    {
        public BackgroundTaskPage()
        {
            InitializeComponent();
        }

        private void OnRegisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            //// Change the network state to run the background task (e.g. disable WiFi). . 

            var builder = new BackgroundTaskBuilder();
            builder.Name = "MyBackgroundTask";
            builder.TaskEntryPoint = "SampleWindowsStoreApp.BackgroundTask.MyBackgroundTask";
            builder.SetTrigger(new SystemTrigger(SystemTriggerType.NetworkStateChange, false));
            builder.Register();
        }

        private void OnDeregisterBackgroundTask(object sender, RoutedEventArgs e)
        {
            var task = BackgroundTaskRegistration.AllTasks.Values.SingleOrDefault(t => t.Name == "MyBackgroundTask");
            if (task != null)
                task.Unregister(true);
        }
    }
}
