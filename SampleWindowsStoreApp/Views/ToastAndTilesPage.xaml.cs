using System;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using MyToolkit.Messaging;
using MyToolkit.Paging;
using MyToolkit.Utilities;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class ToastAndTilesPage
    {
        public ToastAndTilesPage()
        {
            InitializeComponent();
        }

        #region Toasts

        private void OnShowToast(object sender, RoutedEventArgs e)
        {
            // Important: Enable toast notifications in Package.appxmanifest!

            // Copy code from MyToolkit library if you don't want to use the library
            ToastNotificationHelper.ShowMessage("Hello world!"); 
        }

        #endregion

        #region Secondary Tiles

        private const string MainTileId = "App";
        private const string SecondaryTileId = "MyTileId";

        private async void OnPinToStart(object sender, RoutedEventArgs e)
        {
            var tiles = await SecondaryTile.FindAllAsync();
            
            var tile = tiles.SingleOrDefault(t => t.TileId == SecondaryTileId);
            if (tile == null)
            {
                tile = new SecondaryTile(SecondaryTileId,
                    "Display Name",
                    "Arguments",
                    new Uri("ms-appx:///Assets/Logo.png"),
                    TileSize.Default);

                if (await tile.RequestCreateAsync())
                {
                    // User accepted the tile
                }
            }
            else
            {
                // Secondary tile already pinned
            }
        }

        private void OnUpdateSecondaryTile(object sender, RoutedEventArgs e)
        {
            // See http://msdn.microsoft.com/en-us/library/windows/apps/xaml/hh868253.aspx

            var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);

            var tileTextAttributes = tileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "Hello World! My very own tile notification";

            var tileNotification = new TileNotification(tileXml);
            tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddMinutes(10);
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(SecondaryTileId).Update(tileNotification);
        }

        private async void OnRemoveSecondaryTile(object sender, RoutedEventArgs e)
        {
            var tiles = await SecondaryTile.FindAllAsync();
            
            var tile = tiles.SingleOrDefault(t => t.TileId == SecondaryTileId);
            if (tile != null)
                await tile.RequestDeleteAsync();
        }

        public static void HandleSecondaryTileClick(LaunchActivatedEventArgs args)
        {
            if (args.TileId != MainTileId)
            {
                new TextMessage(string.Format("Secondary tile clicked '{0}' with arguments '{1}'", args.TileId, args.Arguments),
                    "App started", MessageButton.OK).Send();
            }
        }

        protected override void OnNavigatedTo(MtNavigationEventArgs e)
        {
            // TODO: This should be registered in the App's OnLaunched method after creating a new Frame object. 
            Messenger.Default.Register(this, DefaultActions.GetTextMessageAction());
        }

        protected override void OnNavigatedFrom(MtNavigationEventArgs e)
        {
            Messenger.Default.Deregister(this);
        }

        #endregion
    }
}
