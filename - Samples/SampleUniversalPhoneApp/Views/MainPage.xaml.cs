using System;
using System.Collections.Generic;
using Windows.Media.Playback;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Multimedia;

namespace SampleUniversalPhoneApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            YouTubeTextBox.Text = "Zln9I9IttLA";
        }

        private void OnShowDetails(object sender, RoutedEventArgs e)
        {
            Frame.NavigateAsync(typeof(DetailsPage));
        }

        private async void OnPlay(object o, RoutedEventArgs routedEventArgs)
        {
            var youTubeId = YouTubeTextBox.Text;
            try
            {
                //// TODO: Show progress bar
                var uri = await YouTube.GetVideoUriAsync(youTubeId, YouTubeQuality.Quality1080P);
                if (uri != null)
                {
                    // @"MICROSOFTVIDEO://www.example.com/myFile.wmv";
                    //var options = new LauncherOptions();
                    //options.ContentType = "video/mp4";
                    //await Launcher.LaunchUriAsync(uri.Uri, options);

                    var player = BackgroundMediaPlayer.Current;
                    player.SetUriSource(uri.Uri);
                    player.Play();
                }
                else
                    throw new Exception("no_video_urls_found");
            }
            catch (Exception exception)
            {
                //// TODO: Show exception
            }
            finally
            {
                //// TODO: Hide progress bar
            }
        }
    }
}
