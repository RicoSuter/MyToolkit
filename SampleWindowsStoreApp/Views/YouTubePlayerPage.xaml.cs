using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using MyToolkit.Multimedia;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class YouTubePlayerPage
    {
        // See http://mytoolkit.codeplex.com/wikipage?title=YouTube

        public YouTubePlayerPage()
        {
            InitializeComponent();
            YouTubeIdBox.Text = "Da1E_VQTNTI";
        }

        private async void OnPlayYouTubeVideo(object sender, RoutedEventArgs e)
        {
            try
            {
                Progress.IsActive = true;

                var uri = await YouTube.GetVideoUriAsync(YouTubeIdBox.Text, YouTubeQuality.Quality720P);
                if (uri != null)
                {
                    YouTubePlayer.Source = uri.Uri;
                    YouTubePlayer.Play();
                }
                else
                {
                    Debugger.Break(); // TODO: Show error message (no video uri found)
                    Progress.IsActive = false;
                }
            }
            catch (Exception exception)
            {
                // TODO: Add exception handling
                Debugger.Break();
                Progress.IsActive = false;
            }
        }

        private void OnMediaOpened(object sender, RoutedEventArgs e)
        {
            Progress.IsActive = false;
        }

        private void OnMediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            // TODO: Display error message
            Progress.IsActive = false; 
        }
    }
}
