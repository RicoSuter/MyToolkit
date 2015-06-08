using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Paging;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class NavigationSamplePage
    {
        public NavigationSamplePage()
        {
            InitializeComponent();
        }

        /// <summary>Called when navigated to this page. </summary>
        /// <param name="args">The event arguments. </param>
        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            TextBox.Text = Frame.BackStackDepth.ToString();
            base.OnNavigatedTo(args);
        }

        /// <summary>Called when navigating from this page. 
        /// The navigation does no happen until the returned task has completed. 
        /// Return null or empty task to run the method synchronously. </summary>
        /// <param name="args">The event arguments. </param>
        /// <returns>The task. </returns>
        protected override Task OnNavigatingFromAsync(MtNavigatingCancelEventArgs args)
        {
            if (args.NavigationMode == NavigationMode.Back)
                return Task.Delay(500);
            return Task.Delay(0);
        }

        /// <summary>Called when navigated from this page. </summary>
        /// <param name="args">The event arguments. </param>
        protected override void OnNavigatedFrom(MtNavigationEventArgs args)
        {
            base.OnNavigatedFrom(args);
        }

        private async void OnNavigateToPage(object sender, RoutedEventArgs e)
        {
            await Frame.NavigateAsync(typeof (NavigationSamplePage));
        }

        private async void OnNavigateHome(object sender, RoutedEventArgs e)
        {
            await Frame.GoHomeAsync();
        }
    }
}
