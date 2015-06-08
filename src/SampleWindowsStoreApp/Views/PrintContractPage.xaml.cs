using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.UI.Xaml.Printing;
using MyToolkit.Paging;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class PrintContractPage
    {
        private PrintDocument _printDocument;
        private IPrintDocumentSource _documentSource;

        public PrintContractPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            PrintManager.GetForCurrentView().PrintTaskRequested += OnPrintManagerPrintTaskRequested;

            _printDocument = new PrintDocument();
            _printDocument.Paginate += OnPrintDocumentPaginate;
            _printDocument.GetPreviewPage += OnPrintDocumentGetPreviewPage;
            _printDocument.AddPages += OnPrintDocumentAddPages;

            _documentSource = _printDocument.DocumentSource;
        }

        protected override void OnNavigatedFrom(MtNavigationEventArgs args)
        {
            PrintManager.GetForCurrentView().PrintTaskRequested -= OnPrintManagerPrintTaskRequested;
        }

        private void OnPrintManagerPrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var printTask = args.Request.CreatePrintTask("Sample Document", OnPrintTaskSourceRequested);
            GenerateOptions(printTask);
        }

        private void GenerateOptions(PrintTask printTask)
        {
            var optionDetails = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTask.Options);
            
            var listOption1 = optionDetails.CreateItemListOption("listOption1", "List Option 1");
            listOption1.AddItem("Foo", "Foo");
            listOption1.AddItem("Bar", "Bar");

            var textOption1 = optionDetails.CreateTextOption("textOption1", "Text Option 1");

            optionDetails.DisplayedOptions.Add("listOption1");
            optionDetails.DisplayedOptions.Add("textOption1");

            optionDetails.Options["listOption1"].TrySetValue("Bar");
            optionDetails.Options[StandardPrintTaskOptions.Orientation].TrySetValue(PrintOrientation.Landscape);

            optionDetails.OptionChanged += (details, e) =>
            {
                // TODO Store options to use when generating document
            };
        }

        private void OnPrintTaskSourceRequested(PrintTaskSourceRequestedArgs args)
        {
            args.SetSource(_documentSource);
        }

        private void OnPrintDocumentPaginate(object sender, PaginateEventArgs args)
        {
            _printDocument.SetPreviewPageCount(2, PreviewPageCountType.Final);
        }

        private void OnPrintDocumentGetPreviewPage(object sender, GetPreviewPageEventArgs args)
        {
            _printDocument.SetPreviewPage(args.PageNumber, args.PageNumber == 0 ? Page1 : Page2);
        }

        private void OnPrintDocumentAddPages(object sender, AddPagesEventArgs args)
        {
            _printDocument.AddPage(Page1);
            _printDocument.AddPage(Page2);
            _printDocument.AddPagesComplete();
        }
    }
}
