using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Multimedia;
using MyToolkit.UI.Popups;
using SampleWindowsStoreApp.ViewModels;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class VariousSamplesPage
    {
		public VariousSamplesPageModel Model {get { return (VariousSamplesPageModel) Resources["viewModel"]; }}

        public VariousSamplesPage()
        {
            InitializeComponent();
        }

	    private async void OnListPickerBoxTest(object sender, RoutedEventArgs e)
	    {
			var list = new [] { "a", "b", "c", "d" };
			var selected = new [] { "a", "b" };
			await ListPickerBox.ShowAsync("header", list, selected, true, true);
	    }
    }
}
