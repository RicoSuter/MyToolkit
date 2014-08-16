//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using MyToolkit.Paging;
using SampleWindowsStoreApp.Views;

namespace SampleWindowsStoreApp
{
    sealed partial class App : MtApplication
    {
        public App()
        {
            InitializeComponent();
        }

        public override Type StartPageType
        {
            get { return typeof (WelcomePage); }
        }

        public override Task OnInitializedAsync(MtFrame frame, ApplicationExecutionState args)
        {
            // TODO: Called when the app is started (not resumed)
            return null;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            await InitializeFrameAsync(args.PreviousExecutionState);

            // TODO: Remove these calls if not used
            ToastAndTilesPage.HandleSecondaryTileClick(args);
        }

        #region Search Contract

        protected override async void OnSearchActivated(SearchActivatedEventArgs args)
        {
            // TODO: Add "Search" in the "Declarations" section of Package.appxmanifest
            await InitializeFrameAsync(args.PreviousExecutionState);
            
            // TODO: Implement this directly in App.xaml.cs
            SearchSamplePage.OnSearchActivated(args);
        }

        #endregion

        #region File Extension

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            await InitializeFrameAsync(args.PreviousExecutionState);

            // TODO: Implement this directly in App.xaml.cs
            FilePickerPage.OnFileActivated(args);
        }

        #endregion

        #region Share Target

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            await InitializeFrameAsync(args.PreviousExecutionState);

            // TODO: Implement this directly in App.xaml.cs
            ShareTargetPage.OnShareTargetActivated(args);
        }

        #endregion
    }
}
