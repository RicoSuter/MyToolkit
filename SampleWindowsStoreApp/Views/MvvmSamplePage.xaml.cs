using System;
using System.ComponentModel;
using Windows.ApplicationModel.DataTransfer;
using MyToolkit.Data;
using MyToolkit.Messaging;
using MyToolkit.Model;
using MyToolkit.MVVM;
using MyToolkit.Paging;
using SampleWindowsStoreApp.ViewModels;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class MvvmSamplePage
    {
        // See http://mytoolkit.codeplex.com/wikipage?title=MVVM%20Overview

        public MvvmSamplePageModel Model
        {
            get { return (MvvmSamplePageModel) Resources["ViewModel"]; }
        }

        public MvvmSamplePage()
        {
            InitializeComponent();
            Model.PropertyChanged += OnPropertyChanged;
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

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            // Sample of strongly typed property changed checking (MyToolkit only)
            if (args.IsProperty<MvvmSamplePageModel>(i => i.DependentProperty))
            {
                // TODO: Add logic for handling DependentProperty change
            }
        }
    }
}
