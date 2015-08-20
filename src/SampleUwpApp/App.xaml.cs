using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Controls;
using MyToolkit.Paging;
using SampleUwpApp.Views;

namespace SampleUwpApp
{
    sealed partial class App : MtApplication
    {
        private HamburgerFrame _hamburgerFrame;

        public App()
        {
            InitializeComponent();
        }

        public override Type StartPageType => typeof(MainPage);

        public override UIElement CreateWindowContentElement()
        {
            _hamburgerFrame = new HamburgerFrame();
            _hamburgerFrame.Hamburger.Header = new HamburgerHeader();
            _hamburgerFrame.Hamburger.TopItems = new ObservableCollection<HamburgerItem>
            {
                new PageHamburgerItem
                {
                    Content = "Home",
                    Icon = '\uE10F'.ToString(),
                    PageType = typeof(MainPage)
                },
                new PageHamburgerItem
                {
                    Content = "Test",
                    Icon = '\uE13D'.ToString(),
                    PageType = typeof(TestPage)
                }
            };
            _hamburgerFrame.Hamburger.BottomItems = new ObservableCollection<HamburgerItem>
            {
                new SearchHamburgerItem(),
                new PageHamburgerItem
                {
                    Content = "Settings",
                    Icon = '\uE115'.ToString(),
                    PageType = typeof(SettingsPage)
                }
            };
            return _hamburgerFrame.Hamburger;
        }

        public override MtFrame GetFrame(UIElement windowContentElement)
        {
            return _hamburgerFrame.Frame;
        }
    }
}
