using System.ComponentModel;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MyToolkit.Multimedia;
using SamplePhoneApp.ViewModels;

// see http://mytoolkit.codeplex.com/wikipage?title=YouTube

namespace SamplePhoneApp.Views
{
	public partial class YouTubePage : PhoneApplicationPage
	{
		public YouTubePageViewModel Model { get { return (YouTubePageViewModel)Resources["viewModel"]; } }

		public YouTubePage()
		{
			InitializeComponent();
		}

		protected override void OnBackKeyPress(CancelEventArgs e)
		{
			if (YouTube.CancelPlay()) 
				e.Cancel = true;
			base.OnBackKeyPress(e);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			YouTube.CancelPlay();
			Model.IsLoading = false;
		}

		private void OnPlay(object sender, RoutedEventArgs args)
		{
			Model.IsLoading = true;
			YouTube.Play("hiWoQpWuKMg", true, YouTubeQuality.Quality480P, e =>
			{
				if (e != null)
					MessageBox.Show(e.Message);
			});
		}
	}
}