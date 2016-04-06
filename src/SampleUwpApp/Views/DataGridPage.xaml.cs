using System.ComponentModel;
using MyToolkit.Model;
using MyToolkit.Paging;
using SampleUwpApp.ViewModels;

namespace SampleUwpApp.Views
{
    public sealed partial class DataGridPage
    {
        public DataGridPage()
        {
            InitializeComponent();
            Model.PropertyChanged += (sender, args) =>
            {
                if (args.IsProperty<DataGridPageModel>(m => m.Filter))
                {
                    DataGrid.SetFilter<Person>(p =>
                        p.FirstName.ToLower().Contains(Model.Filter.ToLower()) ||
                        p.LastName.ToLower().Contains(Model.Filter.ToLower()) ||
                        p.Category.ToLower().Contains(Model.Filter.ToLower()));
                }
            };
            DataGrid.OrderChanged += (sender, args) =>
            {
                Status.Text = string.Format(
                    "Items are ordered by {0} ({1})",
                    args.Column.OrderPropertyPath.Path,
                    (args.Column.IsAscending ? "ascending" : "descending")
                );
            };
        }

        public DataGridPageModel Model
        {
            get { return (DataGridPageModel) Resources["ViewModel"]; }
        }

        protected override void OnSaveState(MtSaveStateEventArgs pageState)
        {
            pageState.Set("Filter", Model.Filter);
        }

        protected override void OnLoadState(MtLoadStateEventArgs pageState)
        {
            Model.Filter = pageState.Get<string>("Filter");
        }
    }
}
