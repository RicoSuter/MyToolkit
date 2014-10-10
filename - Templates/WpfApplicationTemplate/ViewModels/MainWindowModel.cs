using System.Windows.Input;
using MyToolkit.Command;
using MyToolkit.Messaging;
using MyToolkit.Mvvm;

namespace WpfApplicationTemplate.ViewModels
{
    /// <summary>The main window view model. </summary>
    public class MainWindowModel : ViewModelBase
    {
        /// <summary>Initializes a new instance of the <see cref="MainWindowModel"/> class. </summary>
        public MainWindowModel()
        {
            ShowDialogCommand = new RelayCommand(ShowDialog);
        }

        /// <summary>Gets the command to show a dialog message. </summary>
        public ICommand ShowDialogCommand { get; private set; }

        /// <summary>Initializes the view model. Must only be called once per view model instance 
        /// (after the InitializeComponent method of a UserControl). </summary>
        public override void Initialize()
        {
            // TODO: Add your view model initialization logic here. 
            base.Initialize();
        }

        private async void ShowDialog()
        {
            await Messenger.Default.SendAsync(new TextMessage("Hello world!"));
        }
    }
}
