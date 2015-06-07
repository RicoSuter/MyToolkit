using System.Linq;
using Windows.UI.Xaml;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class MtPivotPage
    {
        public MtPivotPage()
        {
            InitializeComponent();
        }

        private void OnRemoveLastPivotItem(object sender, RoutedEventArgs e)
        {
            var item = Pivot.Items.LastOrDefault();
            if (item != null)
                Pivot.Items.Remove(item);
        }
    }
}
