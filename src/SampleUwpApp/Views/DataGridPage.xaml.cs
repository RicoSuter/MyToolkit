using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace SampleUwpApp.Views
{
    public sealed partial class DataGridPage
    {
        public DataGridPage()
        {
            InitializeComponent();

            var list = new List<object>
            {
                new {FirstName = "John", LastName = "Doe", Category = "A"},
                new {FirstName = "Max", LastName = "Muster", Category = "B"},
            };

            for (int i = 0; i < 30; i++)
                list.Add(new { FirstName = "Foo", LastName = "Bar", Category = "C" + i });

            DataGrid.ItemsSource = list;
        }
    }
}
