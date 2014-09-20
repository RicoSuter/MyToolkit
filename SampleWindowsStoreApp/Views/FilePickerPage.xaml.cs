using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.UI.Xaml;
using MyToolkit.Paging;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class FilePickerPage
    {
        public FilePickerPage()
        {
            InitializeComponent();
        }

        private async void OnSaveFile(object sender, RoutedEventArgs e)
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Text Document", new[] { ".mytxt" });

            picker.DefaultFileExtension = ".mytxt";
            picker.SuggestedFileName = "Document.txt";
            picker.SettingsIdentifier = "MySaveFilePicker";

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                // Used to prevent updates to the remote version of the file 
                // until we finish making changes and call CompleteUpdatesAsync
                CachedFileManager.DeferUpdates(file); 

                await FileIO.WriteTextAsync(file, TextBox.Text);

                var status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                    throw new Exception("File could not be saved: " + status);
            }
        }

        private async void OnLoadFile(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mytxt");
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SettingsIdentifier = "MyOpenFilePicker";
            picker.CommitButtonText = "Open";

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenReadAsync())
                {
                    using (var reader = new StreamReader(stream.AsStream()))
                        TextBox.Text = await reader.ReadToEndAsync();
                }

                //TextBox.Text = await FileIO.ReadTextAsync(file);
            }
        }

        // File extension registration (see Package.appxmanifest for file extension registartion)
        public static async void OnFileActivated(FileActivatedEventArgs args)
        {
            // TODO: Implement this directly in App.xaml.cs

            if (args.Kind == ActivationKind.File)
            {
                var file = args.Files[0];
                if (file.Name.EndsWith(".mytxt"))
                {
                    var frame = (MtFrame)Window.Current.Content;
                    var page = frame.CurrentPage.Page as FilePickerPage;
                    if (page == null)
                    {
                        await frame.NavigateAsync(typeof(FilePickerPage));
                        page = (FilePickerPage)frame.CurrentPage.Page;
                    }

                    page.Initialize(file);
                }
            }
        }

        private async void Initialize(IStorageItem file)
        {
            TextBox.Text = await FileIO.ReadTextAsync((StorageFile) file);
        }
    }
}
