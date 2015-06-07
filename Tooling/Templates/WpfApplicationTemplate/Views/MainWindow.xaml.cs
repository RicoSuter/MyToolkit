using System.Windows;
using MyToolkit.Mvvm;
using WpfApplicationTemplate.ViewModels;

namespace WpfApplicationTemplate.Views
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class. </summary>
        public MainWindow()
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
        }

        /// <summary>Gets the view model. </summary>
        public MainWindowModel Model
        {
            get { return (MainWindowModel) Resources["ViewModel"]; }
        }
    }
}
