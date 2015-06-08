using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Controls;
using MyToolkit.Multimedia;
using MyToolkit.UI.Popups;
using SampleWindowsStoreApp.ViewModels;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class VariousSamplesPage
    {
        public VariousSamplesPageModel Model { get { return (VariousSamplesPageModel)Resources["viewModel"]; } }

        public VariousSamplesPage()
        {
            InitializeComponent();
        }

        private async void OnListPickerBoxTest(object sender, RoutedEventArgs e)
        {
            var list = new[] { "a", "b", "c", "d" };
            var selected = new[] { "a", "b" };
            await ListPickerBox.ShowAsync("header", list, selected, true, true);
        }

        private void OnHtmlTextBlockLoaded(object sender, RoutedEventArgs e)
        {
            HtmlTextBlock = (HtmlTextBlock)sender;
        }

        private void OnChangeFontSize(object sender, RoutedEventArgs e)
        {
            HtmlTextBlock.FontSize += 1;
        }
    }
}
