using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using MyToolkit.Paging;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class ShareTargetPage
    {
        private ShareTargetActivatedEventArgs _shareTargetArgs;

        public ShareTargetPage()
        {
            InitializeComponent();
        }

        // Important: Add share target declaration in application manifest (Package.appxmanifest)

        public static async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            // TODO: Implement this directly in App.xaml.cs
            
            var frame = (MtFrame)Window.Current.Content;
            await frame.NavigateAsync(typeof(ShareTargetPage));
            
            var page = (ShareTargetPage)frame.CurrentPage.Page;
            page.Initialize(args);
        }

        private async void Initialize(ShareTargetActivatedEventArgs args)
        {
            _shareTargetArgs = args; 

            // Load shared data
            args.ShareOperation.ReportStarted();
            var uri = await args.ShareOperation.Data.GetWebLinkAsync();
            args.ShareOperation.ReportDataRetrieved();

            // Show shared data in UI
            UriTextBlock.Text = uri.ToString();
        }

        private async void OnShareUri(object sender, RoutedEventArgs e)
        {
            if (_shareTargetArgs != null)
            {
                // TODO: Share URI and show progress bar until it's finished

                _shareTargetArgs.ShareOperation.ReportCompleted();
                await Frame.GoBackAsync(); 
            }
        }

        private async void OnCancel(object sender, RoutedEventArgs e)
        {
            if (_shareTargetArgs != null)
            {
                _shareTargetArgs.ShareOperation.ReportCompleted();
                await Frame.GoBackAsync(); 
            }
        }
    }
}
