using Windows.ApplicationModel.Background;
using MyToolkit.Utilities;

namespace SampleWindowsStoreApp.BackgroundTask
{
    public sealed class MyBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            ToastNotificationUtilities.ShowMessage("Hello from the background task. ");
            deferral.Complete();
        }
    }
}
