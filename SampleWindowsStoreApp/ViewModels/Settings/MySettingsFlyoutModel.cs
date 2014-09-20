using MyToolkit.MVVM;
using MyToolkit.Mvvm;
using SampleWindowsStoreApp.Views.Settings;

namespace SampleWindowsStoreApp.ViewModels.Settings
{
    public class MySettingsFlyoutModel : ViewModelBase
	{
        public ApplicationSettings Settings { get { return new ApplicationSettings(); } }
	}
}
