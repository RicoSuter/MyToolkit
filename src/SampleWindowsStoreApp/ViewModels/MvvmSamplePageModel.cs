using System;
using System.Windows.Input;
using MyToolkit.Command;
using MyToolkit.Messaging;
using MyToolkit.MVVM;
using MyToolkit.Mvvm;
using MyToolkit.Paging;

namespace SampleWindowsStoreApp.ViewModels
{
    public class MvvmSamplePageModel : ViewModelBase, IStateHandlingViewModel
    {
        public MvvmSamplePageModel()
        {
            RunSampleCommand = new RelayCommand((Action)RunSample, () => SampleProperty != "Foo");
        }

        #region Properties

        private string _sampleProperty; 

        public string SampleProperty
        {
            get { return _sampleProperty; }
            set
            {
                if (Set(ref _sampleProperty, value))
                {
                    RaisePropertyChanged(() => DependentProperty);
                    RunSampleCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string DependentProperty
        {
            get { return "Original: " + SampleProperty; }
        }

        #endregion

        #region Commands

        public RelayCommand RunSampleCommand { get; private set; }

        private async void RunSample()
        {
            var message = new TextMessage("Set Text to Foo?", "Update Text", MessageButton.YesNo);
            var result = await Messenger.Default.SendAsync(message);

            if (result.Result == MessageResult.Yes)
                SampleProperty = "Foo";
        }

        #endregion

        /// <summary>Used to save the state when the page gets suspended. </summary>
        /// <param name="pageState">The dictionary to save the page state into. </param>
        public void OnSaveState(MtSaveStateEventArgs pageState)
        {
            pageState.Set("SampleProperty", SampleProperty);
        }

        /// <summary>Used to load the saved state when the page has been reactivated. </summary>
        /// <param name="pageState">The saved page state. </param>
        public void OnLoadState(MtLoadStateEventArgs pageState)
        {
            SampleProperty = pageState.Get<string>("SampleProperty");
        }
    }
}
