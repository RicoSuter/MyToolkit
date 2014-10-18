using MyToolkit.Mvvm;
using MyToolkit.MVVM;
using MyToolkit.Paging;

namespace WindowsPhoneApplicationTemplate.ViewModels
{
    public class MainPageModel : ViewModelBase, IStateHandlingViewModel
    {
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
    }
}
