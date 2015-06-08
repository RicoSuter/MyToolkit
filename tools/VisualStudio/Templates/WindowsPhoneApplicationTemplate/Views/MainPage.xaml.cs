using MyToolkit.Paging;
using WindowsPhoneApplicationTemplate.ViewModels;

namespace WindowsPhoneApplicationTemplate.Views
{
    public sealed partial class MainPage : MtPage
    {
        public MainPage()
        {
            InitializeComponent();
            RegisterViewModel(Model, true);
        }

        /// <summary>Gets the view model. </summary>
        public MainPageModel Model
        {
            get { return (MainPageModel) Resources["ViewModel"]; }
        }

        /// <summary>Called when navigated to this page. </summary>
        /// <param name="args">The event arguments. </param>
        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            // TODO: Prepare page for display. 
            base.OnNavigatedTo(args);
        }
    }
}
