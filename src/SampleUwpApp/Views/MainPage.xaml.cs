using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Paging;

namespace SampleUwpApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>Called when navigated to this page. </summary>
        /// <param name="args">The event arguments. </param>
        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            if (args.NavigationMode == NavigationMode.New)
                Test.Text = "Test: " + Frame.BackStackDepth;

            base.OnNavigatedTo(args);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainPage));
        }

        protected override void OnSaveState(MtSaveStateEventArgs pageState)
        {
            pageState.Set("MyTextBox", MyTextBox.Text);
        }

        protected override void OnLoadState(MtLoadStateEventArgs pageState)
        {
            MyTextBox.Text = pageState.Get<string>("MyTextBox");
        }
    }
}
