using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using SampleWindowsStoreApp.Localization;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class LocalizationPage
    {
        public LocalizationPage()
        {
            InitializeComponent();
        }

        private void OnAccessTranslatedStringInCode(object sender, RoutedEventArgs e)
        {
            // Default way: Using key as string
            var loader = new ResourceLoader();
            var str1 = loader.GetString("LocalizedString");

            Debug.WriteLine(str1);

            // Using T4 Template, see http://blog.rsuter.com/?p=439
            var str2 = LocalizedStrings.LocalizedString;

            Debug.WriteLine(str2);
            Debugger.Break();
        }
    }
}
