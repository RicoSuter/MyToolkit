using System;
using System.Windows.Input;
using MyToolkit.Command;
using MyToolkit.Messaging;
using MyToolkit.MVVM;
using MyToolkit.Mvvm;

namespace SampleWindowsStoreApp.ViewModels
{
    public class MvvmSamplePageModel : ViewModelBase
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
    }
}
