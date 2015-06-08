using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using MyToolkit.Paging;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class ShareContractPage
    {
        public ShareContractPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            var manager = DataTransferManager.GetForCurrentView();
            manager.DataRequested += OnShareData;
        }

        protected override void OnNavigatedFrom(MtNavigationEventArgs args)
        {
            var manager = DataTransferManager.GetForCurrentView();
            manager.DataRequested -= OnShareData;
        }

        private void OnShareData(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            var deferral = request.GetDeferral();

            // TODO: Set shared data
            request.Data.Properties.Title = "Share Sample Text";
            request.Data.Properties.Description = "Share Sample Text to Application";
            request.Data.SetDataProvider(StandardDataFormats.Bitmap, OnShareBitmapData);

            deferral.Complete();
        }

        private void OnShareBitmapData(DataProviderRequest request)
        {
            var deferral = request.GetDeferral();
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var file = await ApplicationData.Current.TemporaryFolder.
                        CreateFileAsync("TempImage.png",
                        CreationCollisionOption.ReplaceExisting);

                    // Render XAML control to PNG image
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var renderTargetBitmap = new RenderTargetBitmap();
                        await renderTargetBitmap.RenderAsync(SharedControl);
                        var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();

                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                        encoder.SetPixelData(
                            BitmapPixelFormat.Bgra8,
                            BitmapAlphaMode.Straight,
                            (uint)renderTargetBitmap.PixelWidth,
                            (uint)renderTargetBitmap.PixelHeight,
                            96, 96,
                            pixelBuffer.ToArray());

                        await encoder.FlushAsync();
                    }

                    // Share file
                    request.SetData(RandomAccessStreamReference.CreateFromFile(file));
                }
                finally
                {
                    deferral.Complete();
                }
            });
        }
    }
}
