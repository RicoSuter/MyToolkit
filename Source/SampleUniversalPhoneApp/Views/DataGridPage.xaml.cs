using System.Collections.Generic;
using MyToolkit.Paging;
using SampleUniversalPhoneApp.Models;

namespace SampleUniversalPhoneApp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataGridPage
    {
        public DataGridPage()
        {
            InitializeComponent();

            DataGrid.ItemsSource = new List<Person>
            {
                new Person {Firstname = "a", Lastname = "b", Category = "c"},
                new Person {Firstname = "d", Lastname = "e", Category = "f"},
            };
        }

        /// <summary>Invoked when this page is about to be displayed in a Frame. </summary>
        /// <param name="args">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
        }
    }
}
