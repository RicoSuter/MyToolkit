using System;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyToolkit.Controls;
using SamplePhoneApp.Domain;
using SamplePhoneApp.Resources;
using SamplePhoneApp.ViewModels;

namespace SamplePhoneApp.Views
{
	public partial class ListPage : PhoneApplicationPage
	{
		public bool IsNewPage { get; private set; }
		public ListPageViewModel Model { get { return (ListPageViewModel)Resources["viewModel"]; } }

		public ListPage()
		{
			InitializeComponent();
			IsNewPage = true;
		}

		protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (IsNewPage)
			{
				((ApplicationBarIconButton) ApplicationBar.Buttons[0]).Text = Strings.AppbarAdd; 
				IsNewPage = false;
			}
		}

		private void OnAddPerson(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/DetailsPage.xaml", UriKind.Relative));
		}

		private void OnEditPerson(object sender, NavigationListEventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/DetailsPage.xaml?SelectedItem=" + e.GetItem<Person>().Id, UriKind.Relative));
		}
	}
}