using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Controls;
using MyToolkit.Utilities;
using SampleWindowsStoreApp.Domain;
using SampleWindowsStoreApp.ViewModels;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class DataGridPage
    {
        public DataGridPageModel ViewModel { get { return (DataGridPageModel) Resources["ViewModel"]; } }

        public DataGridPage()
        {
            InitializeComponent();
        }

        private void OnSelectLastPerson(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedPerson = ViewModel.People
                .Where(p => p != ViewModel.SelectedPerson).ToList()
                .TakeRandom(1).Single();
        }

        private DataGridColumn _removedColumn;
        private void OnRemoveFirstColumn(object sender, RoutedEventArgs e)
        {
            if (DataGrid.Columns.Count > 0)
            {
                _removedColumn = DataGrid.Columns.First();
                DataGrid.Columns.Remove(_removedColumn);
            }
        }

        private void OnAddRemovedColumn(object sender, RoutedEventArgs e)
        {
            if (_removedColumn != null)
            {
                DataGrid.Columns.Add(_removedColumn);
                _removedColumn = null; 
            }
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs e)
        {
            var filter = ((TextBox) sender).Text;
            if (string.IsNullOrEmpty(filter))
                DataGrid.RemoveFilter();
            else
            {
                filter = filter.ToLower();
                DataGrid.SetFilter<Person>(p => p.Firstname.ToLower().Contains(filter) || p.Lastname.ToLower().Contains(filter));
            }
        }
    }
}
