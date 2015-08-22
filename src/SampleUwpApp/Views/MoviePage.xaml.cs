using System;
using MyToolkit.Multimedia;
using MyToolkit.Paging;

namespace SampleUwpApp.Views
{
    public sealed partial class MoviePage
    {
        public MoviePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            if (MediaElement.Source != null)
                MediaElement.Play();
        }

        protected override void OnNavigatingFrom(MtNavigatingCancelEventArgs args)
        {
            MediaElement.Pause();
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
