using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyToolkit.Controls;
using MyToolkit.Paging;
using SamplePhoneApp.Domain;
using SamplePhoneApp.Resources;
using SamplePhoneApp.ViewModels;

namespace SamplePhoneApp.Views
{
	public partial class ApplicationBarPage
	{
        public ApplicationBarPage()
		{
			InitializeComponent();
		}

        private void OnHideFirstButton(object sender, RoutedEventArgs e)
        {
            var bar = BindableApplicationBar.GetApplicationBar(this);
            bar.Buttons[0].IsVisible = !bar.Buttons[0].IsVisible;
        }
        
        private void OnHideSecondButton(object sender, RoutedEventArgs e)
	    {
	        var bar = BindableApplicationBar.GetApplicationBar(this);
	        bar.Buttons[1].IsVisible = !bar.Buttons[1].IsVisible;
	    }

	    private void OnDisableSecondButton(object sender, RoutedEventArgs e)
	    {
            var bar = BindableApplicationBar.GetApplicationBar(this);
            bar.Buttons[1].IsEnabled = !bar.Buttons[1].IsEnabled;
        }

	    private void OnDisableThirdButton(object sender, RoutedEventArgs e)
	    {
            var bar = BindableApplicationBar.GetApplicationBar(this);
            bar.Buttons[2].IsEnabled = !bar.Buttons[2].IsEnabled;
        }



        private void OnHideFirstMenu(object sender, RoutedEventArgs e)
        {
            var bar = BindableApplicationBar.GetApplicationBar(this);
            bar.MenuItems[0].IsVisible = !bar.MenuItems[0].IsVisible;
        }

        private void OnHideSecondMenu(object sender, RoutedEventArgs e)
        {
            var bar = BindableApplicationBar.GetApplicationBar(this);
            bar.MenuItems[1].IsVisible = !bar.MenuItems[1].IsVisible;
        }

        private void OnDisableSecondMenu(object sender, RoutedEventArgs e)
        {
            var bar = BindableApplicationBar.GetApplicationBar(this);
            bar.MenuItems[1].IsEnabled = !bar.MenuItems[1].IsEnabled;
        }

        private void OnDisableThirdMenu(object sender, RoutedEventArgs e)
        {
            var bar = BindableApplicationBar.GetApplicationBar(this);
            bar.MenuItems[2].IsEnabled = !bar.MenuItems[2].IsEnabled;
        }
	}
}