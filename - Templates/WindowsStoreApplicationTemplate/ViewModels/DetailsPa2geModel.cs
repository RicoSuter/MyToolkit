using MyToolkit.Mvvm;
using MyToolkit.MVVM;
using MyToolkit.Paging;

namespace WindowsStoreApplicationTemplate.ViewModels
{
    /// <summary>The details page view model. </summary>
    public class DetailsPageModel : ViewModelBase, IStateHandlingViewModel
    {
        /// <summary>Initializes the view model. Must only be called once per view model instance 
        /// (after the InitializeComponent method of a UserControl). </summary>
        public override void Initialize()
        {
            // TODO: Add your view model initialization logic here. 
        }

        /// <summary>Used to save the state when the page gets suspended. </summary>
        /// <param name="pageState">The dictionary to save the page state into. </param>
        public void OnSaveState(MtSaveStateEventArgs pageState)
        {
            // TODO: Save the view model state. 
        }

        /// <summary>Used to load the saved state when the page has been reactivated. </summary>
        /// <param name="pageState">The saved page state. </param>
        public void OnLoadState(MtLoadStateEventArgs pageState)
        {
            // TODO: Load the view model state. 
        }

        public void LoadPerson(int personId)
        {
            // TODO: This is only a sample method and can be removed. 
        }
    }
}
