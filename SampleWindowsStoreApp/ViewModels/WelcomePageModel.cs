using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MyToolkit.Collections;
using MyToolkit.MVVM;
using MyToolkit.Mvvm;
using SampleWindowsStoreApp.Views;

namespace SampleWindowsStoreApp.ViewModels
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public Type PageType { get; set; }
    }

    public class WelcomePageModel : ViewModelBase
    {
        public WelcomePageModel()
        {
            Groups = new ObservableCollection<ExtendedGroup<NavigationItem>>();

            Groups.Add(new ExtendedGroup<NavigationItem>("MyToolkit", new List<NavigationItem>
            {
                new NavigationItem {Title = "Model-View-ViewModel (MVVM)", Subtitle = "MvvmSamplePage", PageType = typeof(MvvmSamplePage)},
                new NavigationItem {Title = "YouTube Player and PlayTo", Subtitle = "YouTubePlayerPage", PageType = typeof(YouTubePlayerPage)},
                new NavigationItem {Title = "DataGrid Control", Subtitle = "DataGridPage", PageType = typeof(DataGridPage)},
                new NavigationItem {Title = "MtPivot Control", Subtitle = "MtPivotPage", PageType = typeof(MtPivotPage)},
                new NavigationItem {Title = "Various Samples", Subtitle = "VariousSamplesPage", PageType = typeof(VariousSamplesPage)},
                new NavigationItem {Title = "RSS Loader", Subtitle = "RssReaderPage", PageType = typeof(RssReaderPage)},
                new NavigationItem {Title = "MtListBox", Subtitle = "MtListBoxPage", PageType = typeof(MtListBoxPage)},
            }));

            Groups.Add(new ExtendedGroup<NavigationItem>("Windows 8", new List<NavigationItem>
            {
                new NavigationItem {Title = "Settings", Subtitle = "SettingsSamplePage", PageType = typeof(SettingsSamplePage)},
                new NavigationItem {Title = "Localization", Subtitle = "LocalizationPage", PageType = typeof(LocalizationPage)},
                new NavigationItem {Title = "HTTP", Subtitle = "HttpPage", PageType = typeof(HttpPage)},
                new NavigationItem {Title = "File Save and Open Picker", Subtitle = "FilePickerPage", PageType = typeof(FilePickerPage)},
                new NavigationItem {Title = "Toast and Tiles", Subtitle = "ToastAndTilesPage", PageType = typeof(ToastAndTilesPage)},
                new NavigationItem {Title = "Share Contract", Subtitle = "ShareContractPage", PageType = typeof(ShareContractPage)},
                new NavigationItem {Title = "Print Contract", Subtitle = "PrintContractPage", PageType = typeof(PrintContractPage)},
                new NavigationItem {Title = "Search", Subtitle = "SearchSamplePage", PageType = typeof(SearchSamplePage)},
                new NavigationItem {Title = "Background Task", Subtitle = "BackgroundTaskPage", PageType = typeof(BackgroundTaskPage)},
                new NavigationItem {Title = "App Bar and Command Bar", Subtitle = "AppBarPage", PageType = typeof(AppBarPage)},
                new NavigationItem {Title = "Push Notifications", Subtitle = "PushNotificationPage", PageType = typeof(PushNotificationPage)},
            }));
        }

        public ObservableCollection<ExtendedGroup<NavigationItem>> Groups { get; private set; }
    }
}
