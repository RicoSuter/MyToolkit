using System;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Multimedia;

namespace SampleUniversalPhoneApp.Views
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

            YouTubeTextBox.Text = "Zln9I9IttLA";
        }

        private void OnShowDataGrid(object sender, RoutedEventArgs e)
        {
            Frame.NavigateAsync(typeof(DataGridPage));
        }

        private void OnShowLongListSelector(object sender, RoutedEventArgs e)
        {
            Frame.NavigateAsync(typeof(LongListSelectorPage));
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
