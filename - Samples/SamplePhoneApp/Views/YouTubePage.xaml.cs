using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using MyToolkit.Multimedia;
using SamplePhoneApp.ViewModels;

// See http://mytoolkit.codeplex.com/wikipage?title=YouTube

namespace SamplePhoneApp.Views
{
    public partial class YouTubePage 
    {
        public YouTubePageViewModel Model { get { return (YouTubePageViewModel)Resources["ViewModel"]; } }

        public YouTubePage()
        {
            InitializeComponent();

            Model.YouTubeId = "xE2MxCv5vVY"; //"Zln9I9IttLA";
            AddBackKeyPressHandler(YouTube.HandleBackKeyPress);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            YouTube.CancelPlay();
            Model.IsLoading = false;
        }

        private async void OnPlay(object sender, RoutedEventArgs args)
        {
            Model.IsLoading = true;
            try
            {
                await YouTube.PlayWithPageDeactivationAsync(Model.YouTubeId, true, YouTubeQuality.Quality720P);
            }
            catch (Exception ex)
            {
                Model.IsLoading = false;
                MessageBox.Show(ex.Message);
            }
        }

        private async void OnLaunchUriInBrowser(object sender, RoutedEventArgs e)
        {
            if (Model.IsLoading)
                return;

            Model.IsLoading = true;
            try
            {
                var uri = await YouTube.GetVideoUriAsync(Model.YouTubeId, YouTubeQuality.Quality720P);

                var task = new WebBrowserTask();
                task.Uri = uri.Uri;
                task.Show();

                Model.IsLoading = false;
            }
            catch (Exception ex)
            {
                Model.IsLoading = false;
                MessageBox.Show(ex.Message);
            }
        }
    }
}