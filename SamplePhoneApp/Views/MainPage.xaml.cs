using System;
using System.Windows;
using MyToolkit.UI.Popups;
using SamplePhoneApp.ViewModels;

namespace SamplePhoneApp.Views
{
	public partial class MainPage 
	{
		public MainPageViewModel Model { get { return (MainPageViewModel)Resources["viewModel"]; } }

		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e, bool isNewPage)
		{
			if (isNewPage)
			{
				
			}
		}

		private void ShowList(object sender, RoutedEventArgs e)
		{
			TryNavigate(new Uri("/Views/ListPage.xaml", UriKind.Relative));
		}

		private void ShowYouTube(object sender, RoutedEventArgs e)
		{
			TryNavigate(new Uri("/Views/YouTubePage.xaml", UriKind.Relative));
		}

		private void ShowSamplePage(object sender, RoutedEventArgs e)
		{
			TryNavigate(new Uri("/Views/SamplePage.xaml", UriKind.Relative));
		}

        private void ShowApplicationBar(object sender, RoutedEventArgs e)
        {
            TryNavigate(new Uri("/Views/ApplicationBarPage.xaml", UriKind.Relative));
        }
		
		private async void ShowListPickerBoxTest(object sender, RoutedEventArgs e)
		{
			var list = new string[] { "a", "b", "c", "d" };
			var selected = new string[] { "a", "b" };
			await ListPickerBox.ShowAsync(list, selected, "header", true, true);
		}

		private async void ShowInputBoxTest(object sender, RoutedEventArgs e)
		{
			var text = await InputBox.ShowAsync("Please enter a text: ", "Enter text", "Initial text", true);
		}
	}
}