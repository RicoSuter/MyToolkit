using WindowsStoreApplicationTemplate.ViewModels;
using MyToolkit.Paging;

namespace WindowsStoreApplicationTemplate.Views
{
    public sealed partial class DetailsPage
    {
        public DetailsPage()
        {
            InitializeComponent();
            RegisterViewModel(Model, true);
        }

        /// <summary>Gets the view model. </summary>
        public DetailsPageModel Model
        {
            get { return (DetailsPageModel)Resources["ViewModel"]; }
        }

        /// <summary>Called when navigated to this page. </summary>
        /// <param name="args">The event arguments. </param>
        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            var personId = args.GetParameter<int>();
            Model.LoadPerson(personId);
        }
    }
}
