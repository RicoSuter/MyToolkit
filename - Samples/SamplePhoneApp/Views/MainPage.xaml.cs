using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using SamplePhoneApp.ViewModels;

namespace SamplePhoneApp.Views
{
	public partial class MainPage : PhoneApplicationPage
	{
		public bool IsNewPage { get; private set; }
		public MainPageViewModel Model { get { return (MainPageViewModel)Resources["viewModel"]; } }

		public MainPage()
		{
			InitializeComponent();
			IsNewPage = true;
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (IsNewPage)
			{
				// TODO initialize page here
				IsNewPage = false;
			}
		}

		private void ShowList(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/ListPage.xaml", UriKind.Relative));
		}

		private void ShowYouTube(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/YouTubePage.xaml", UriKind.Relative));
		}
	}
}