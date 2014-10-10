using System.Windows.Input;

using MyToolkit.Command;
using MyToolkit.Messaging;
using MyToolkit.Mvvm;
using MyToolkit.MVVM;
using MyToolkit.Paging;

using WindowsStoreApplicationTemplate.Models;

namespace WindowsStoreApplicationTemplate.ViewModels
{
    /// <summary>The main page view model. </summary>
    public class MainPageModel : ViewModelBase, IStateHandlingViewModel
    {
        private Person _selectedPerson;

        /// <summary>Initializes a new instance of the <see cref="MainPageModel"/> class. </summary>
        public MainPageModel()
        {
            ShowDetailsCommand = new RelayCommand<Person>(ShowDetails);
        }

        /// <summary>Gets the command to navigate to the details page. </summary>
        public ICommand ShowDetailsCommand { get; private set; }

        /// <summary>Gets or sets the selected person. </summary>
        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set { Set(ref _selectedPerson, value); }
        }

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

        private void ShowDetails(Person person)
        {
            Messenger.Default.Send(new NavigateMessage(typeof(DetailsPageModel), person.Id));
        }
    }
}
