using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyToolkit.Controls;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class ExtendedListBoxPage
    {
        private readonly ObservableCollection<string> _list;

        public ExtendedListBoxPage()
        {
            InitializeComponent();

            _list = new ObservableCollection<string>();
            for (var i = 1; i <= 100; i++)
                _list.Add("Item " + i);

            ExtendedListBox.ItemsSource = _list; 
        }

        private async void OnScrolledToEnd(object sender, ScrolledToEndEventArgs e)
        {
            ExtendedListBox.TriggerScrolledToEndEvents = false;
           
            await Task.Delay(2000); // Simulate network
            for (var i = 1; i <= 100; i++)
                _list.Add("Item " + i);

            ExtendedListBox.TriggerScrolledToEndEvents = true;
        }
    }
}
