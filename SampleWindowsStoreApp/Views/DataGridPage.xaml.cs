using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Controls;
using MyToolkit.Paging;
using MyToolkit.Utilities;
using SampleWindowsStoreApp.Models;
using SampleWindowsStoreApp.ViewModels;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class DataGridPage
    {
        private DataGridColumnBase _removedColumn;

        public DataGridPage()
        {
            InitializeComponent();
        }

        public DataGridPageModel ViewModel { get { return (DataGridPageModel)Resources["ViewModel"]; } }
        
        private void OnSelectLastPerson(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedPerson = ViewModel.People
                .Where(p => p != ViewModel.SelectedPerson).ToList()
                .TakeRandom(1).Single();
        }

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
            var filter = ((TextBox)sender).Text;
            if (string.IsNullOrEmpty(filter))
                DataGrid.RemoveFilter();
            else
            {
                filter = filter.ToLower();
                DataGrid.SetFilter<Person>(p => p.Firstname.ToLower().Contains(filter) || p.Lastname.ToLower().Contains(filter));
            }
        }

        private void OnSelectedItemsChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItems.Text = string.Join(", ", DataGrid.SelectedItems.OfType<Person>().Select(p => p.Firstname));
        }

        private void OnAdd10Elements(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                ViewModel.People.Add(new Person
                {
                    Firstname = "Firstname" + i,
                    Lastname = "Lastname" + i,
                    Category = "Category" + i
                });
            }
        }
    }
}
