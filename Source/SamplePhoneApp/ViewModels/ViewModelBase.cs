using MyToolkit.Model;
using SamplePhoneApp.Resources;

namespace SamplePhoneApp.ViewModels
{
	public class ViewModelBase : ObservableObject
	{
		private static readonly Strings _strings = new Strings();
		public Strings Strings { get { return _strings; } }

		private bool _isLoading;
		public bool IsLoading
		{
			get { return _isLoading; }
			set { Set(ref _isLoading, value); }
		}
	}
}
