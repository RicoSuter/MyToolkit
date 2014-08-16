using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using MyToolkit.Storage;
using MyToolkit.Utilities;
using MyToolkit.Multimedia;
using System.Diagnostics;
using MyToolkit.Networking;

namespace SamplePhone7App
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //// Check whether all libraries can be used. 

            ApplicationSettings.SetSetting("Test", "600", true, true);
            Debug.WriteLine(ApplicationSettings.GetSetting("Test", "300", true));

            var color = ColorUtility.ToHex(Colors.Green);
            Debug.WriteLine(color);

            var uri = await YouTube.GetVideoUriAsync("pYVQxKZ6-6Y", YouTubeQuality.Quality720P);
            Debug.WriteLine(uri.Uri);

            var response = await Http.GetAsync("http://www.google.ch");
            Debug.WriteLine(response.Response);
        }
    }
}