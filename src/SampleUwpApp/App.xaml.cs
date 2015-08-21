using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Controls;
using MyToolkit.Paging;
using SampleUwpApp.Views;
using AppBarButton = Windows.UI.Xaml.Controls.AppBarButton;

namespace SampleUwpApp
{
    sealed partial class App : MtApplication
    {
        private HamburgerFrameFactory _hamburgerFrameFactory;

        public App()
        {
            InitializeComponent();
        }

        public override Type StartPageType => typeof(MainPage);

        public override UIElement CreateWindowContentElement()
        {
            _hamburgerFrameFactory = new HamburgerFrameFactory();
            _hamburgerFrameFactory.Hamburger.Header = new HamburgerHeader();
            _hamburgerFrameFactory.Hamburger.TopItems = new ObservableCollection<HamburgerItem>
            {
                new PageHamburgerItem
                {
                    Content = "Home",
                    ContentIcon = new SymbolIcon(Symbol.Home),
                    Icon = new SymbolIcon(Symbol.Home),
                    PageType = typeof(MainPage)
                },
                new SearchHamburgerItem
                {
                    PlaceholderText = "Search"
                },
                new PageHamburgerItem
                {
                    Content = "Movie",
                    ContentIcon = new SymbolIcon(Symbol.Video),
                    Icon = new SymbolIcon(Symbol.Video),
                    PageType = typeof(MoviePage)
                }
            };
            _hamburgerFrameFactory.Hamburger.BottomItems = new ObservableCollection<HamburgerItem>
            {
                new PageHamburgerItem
                {
                    Content = "Settings",
                    ContentIcon = new SymbolIcon(Symbol.Setting),
                    Icon = new SymbolIcon(Symbol.Setting),
                    PageType = typeof(SettingsPage)
                }
            };
            return _hamburgerFrameFactory.Hamburger;
        }

        public override MtFrame GetFrame(UIElement windowContentElement)
        {
            return _hamburgerFrameFactory.Frame;
        }
    }
}
