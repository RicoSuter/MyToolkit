using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyToolkit.Command;
using MyToolkit.Model;
using MyToolkit.Paging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace SampleUwpApp.Views
{
    public sealed partial class HamburgerShell : UserControl
    {
        public HamburgerShell()
        {
            InitializeComponent();

            TopItems = new ObservableCollection<HamburgerItem>();
            BottomItems = new ObservableCollection<HamburgerItem>();

            DataContext = this;
            Frame.PageAnimation = null;

            Frame.Navigated += FrameOnNavigated;
        }

        private bool _changing = false; 

        private void FrameOnNavigated(object sender, MtNavigationEventArgs args)
        {
            _changing = true; 

            var item = TopItems.FirstOrDefault(i => i.PageType == Frame.CurrentPage.Type);
            if (item != null)
                item.IsChecked = true;

            _changing = false;
        }

        private void OnTogglePane(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        public ObservableCollection<HamburgerItem> TopItems { get; private set; }

        public ObservableCollection<HamburgerItem> BottomItems { get; private set; }

        public MtFrame Frame => RootFrame;

        private async void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_changing)
                return;

            var item = (HamburgerItem)((RadioButton) sender).CommandParameter;
            if (item != null)
                await Frame.NavigateAsync(item.PageType, item.PageParameter);
        }
    }

    public class HamburgerItem : ObservableObject
    {
        private bool _isChecked;
        public string Icon { get; set; }

        public string Label { get; set; }

        public Type PageType { get; set; }

        public object PageParameter { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { Set(ref _isChecked, value); }
        }
    }
}
