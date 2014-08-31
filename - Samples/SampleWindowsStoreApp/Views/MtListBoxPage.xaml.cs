using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MyToolkit.Controls;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class MtListBoxPage
    {
        private readonly ObservableCollection<string> _list;

        public MtListBoxPage()
        {
            InitializeComponent();

            _list = new ObservableCollection<string>();
            for (var i = 1; i <= 100; i++)
                _list.Add("Item " + i);

            MtListBox.ItemsSource = _list; 
        }

        private async void OnScrolledToEnd(object sender, ScrolledToEndEventArgs e)
        {
            MtListBox.TriggerScrolledToEndEvents = false;
           
            await Task.Delay(2000); // Simulate network
            for (var i = 1; i <= 100; i++)
                _list.Add("Item " + i);

            MtListBox.TriggerScrolledToEndEvents = true;
        }
    }
}
