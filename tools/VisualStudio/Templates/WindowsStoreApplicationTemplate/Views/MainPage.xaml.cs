using WindowsStoreApplicationTemplate.Models;
using MyToolkit.Paging;

using WindowsStoreApplicationTemplate.ViewModels;

namespace WindowsStoreApplicationTemplate.Views
{
    /// <summary>The main page. </summary>
    public sealed partial class MainPage : MtPage
    {
        /// <summary>Initializes a new instance of the <see cref="MainPage"/> class. </summary>
        public MainPage()
        {
            InitializeComponent();
            RegisterViewModel(Model, true);
        }

        /// <summary>Gets the view model. </summary>
        public MainPageModel Model
        {
            get { return (MainPageModel)Resources["ViewModel"]; }
        }

        /// <summary>Called when navigated to this page. </summary>
        /// <param name="args">The event arguments. </param>
        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            // TODO: Prepare page for display.
            Model.SelectedPerson = new Person { Id = 123 }; // sample code
        }
    }
}
