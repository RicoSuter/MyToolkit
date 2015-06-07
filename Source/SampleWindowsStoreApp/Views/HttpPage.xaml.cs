using System;
using System.Net.Http;
using System.Text;
using Windows.UI.Xaml;
using MyToolkit.Networking;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class HttpPage
    {
        public HttpPage()
        {
            InitializeComponent();
        }

        // See http://mytoolkit.codeplex.com/wikipage?title=Http

        private async void OnHttpGetRequestMyToolkit(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await Http.GetAsync(UrlBox.Text);
                ResponseBox.Text = response.Response;
            }
            catch (OperationCanceledException exception)
            {
                // Do nothing
            }
            catch (HttpStatusException exception)
            {
                // TODO: Handle exception
            }
            catch (Exception exception)
            {
                // TODO: Handle exception
            }
        }

        private async void OnHttpPostRequestMyToolkit(object sender, RoutedEventArgs e)
        {
            try
            {
                var request = new HttpPostRequest(UrlBox.Text);
                request.Data.Add("Key1", "Value1");
                //request.Files.Add(new HttpPostFile(...));
                var response = await request.PostAsync();
                ResponseBox.Text = response.Response;
            }
            catch (OperationCanceledException exception)
            {
                // Do nothing
            }
            catch (HttpStatusException exception)
            {
                // TODO: Handle exception
            }
            catch (Exception exception)
            {
                // TODO: Handle exception
            }
        }


        // See http://msdn.microsoft.com/library/windows/apps/dn298639

        private async void OnHttpGetRequest(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = new Uri(UrlBox.Text);
                var client = new HttpClient();
                ResponseBox.Text = await client.GetStringAsync(uri);
            }
            catch (OperationCanceledException exception)
            {
                // Do nothing
            }
            catch (Exception exception)
            {
                // TODO: Handle exception
            }
        }

        private async void OnHttpPostRequest(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = new Uri(UrlBox.Text);
                var client = new HttpClient();

                var dataString = string.Format("{0}={1}&", Uri.EscapeDataString("Key1"), Uri.EscapeDataString("Value1"));
                var content = new ByteArrayContent(Encoding.UTF8.GetBytes(dataString)) as HttpContent;
                var result = await client.PostAsync(uri, content);

                ResponseBox.Text = await result.Content.ReadAsStringAsync();
            }
            catch (OperationCanceledException exception)
            {
                // Do nothing
            }
            catch (Exception exception)
            {
                // TODO: Handle exception
            }
        }
    }
}
