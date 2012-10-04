using MyToolkit.MVVM;
using SamplePhoneApp.Resources;

namespace SamplePhoneApp.ViewModels
{
	public class BaseViewModel<T> : NotifyPropertyChanged<T> where T : BaseViewModel<T> 
	{
		private static readonly Strings strings = new Strings();
		public Strings Strings { get { return strings; } }

		private bool isLoading;
		public bool IsLoading
		{
			get { return isLoading; }
			set { SetProperty(m => m.IsLoading, ref isLoading, value); }
		}
	}
}
