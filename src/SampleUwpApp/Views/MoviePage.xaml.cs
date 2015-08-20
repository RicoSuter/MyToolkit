using System;
using MyToolkit.Multimedia;

namespace SampleUwpApp.Views
{
    public sealed partial class MoviePage
    {
        public MoviePage()
        {
            InitializeComponent();
        }

        private async void OnPlayMovie(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            try
            {
                var url = await YouTube.GetVideoUriAsync("Gu6vmNz-PhE", YouTubeQuality.QualityLow);
                MediaElement.Source = url.Uri;
                MediaElement.Play();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
