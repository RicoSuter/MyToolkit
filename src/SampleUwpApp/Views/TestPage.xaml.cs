using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Multimedia;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SampleUwpApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TestPage
    {
        public TestPage()
        {
            this.InitializeComponent();
            LoadVideo();
        }

        private async void LoadVideo()
        {
            try
            {
                var url = await YouTube.GetVideoUriAsync("Z-TQ98mRAiw", YouTubeQuality.QualityLow);
                MediaElement.Source = url.Uri;
                MediaElement.Play();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
