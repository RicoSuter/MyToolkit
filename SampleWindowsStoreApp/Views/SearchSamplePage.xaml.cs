using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MyToolkit.Paging;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class SearchSamplePage
    {
        public SearchSamplePage()
        {
            InitializeComponent();

            SearchBox.QueryChanged += OnQueryChanged;
            SearchBox.QuerySubmitted += OnQuerySubmitted;
            SearchBox.ResultSuggestionChosen += OnResultSuggestionChosen;
            SearchBox.SuggestionsRequested += OnSuggestionsRequested;
        }

        public string SearchQuery
        {
            get { return SearchBox.QueryText; }
            set { SearchBox.QueryText = value; }
        }

        private void OnSuggestionsRequested(SearchBox searchBox, SearchBoxSuggestionsRequestedEventArgs args)
        {
            args.Request.SearchSuggestionCollection.AppendQuerySuggestion("Foo");
            args.Request.SearchSuggestionCollection.AppendQuerySuggestion("Bar");
        }

        private void OnQueryChanged(SearchBox searchBox, SearchBoxQueryChangedEventArgs args)
        {

        }

        private void OnResultSuggestionChosen(SearchBox searchBox, SearchBoxResultSuggestionChosenEventArgs args)
        {

        }

        private void OnQuerySubmitted(SearchBox searchBox, SearchBoxQuerySubmittedEventArgs args)
        {
            RequestedSearchQuery.Text = args.QueryText;

            // TODO: Load results and show in page
        }

        // Search contract implementation
        public static async void OnSearchActivated(SearchActivatedEventArgs args)
        {
            // TODO: Implement this directly in App.xaml.cs

            var frame = (MtFrame)Window.Current.Content;
            var page = frame.CurrentPage.Page as SearchSamplePage;
            if (page == null)
            {
                await frame.NavigateAsync(typeof(SearchSamplePage));
                page = (SearchSamplePage)frame.CurrentPage.Page;
            }

            page.Initialize(args);
        }

        private void Initialize(SearchActivatedEventArgs args)
        {
            RequestedSearchQuery.Text = args.QueryText;
        }
    }
}
