using SamplePhoneApp.Domain;

namespace SamplePhoneApp.ViewModels
{
	public class DetailsPageViewModel : ViewModelBase
	{
		private Person _person;
		public Person Person
		{
			get { return _person; }
			set { Set(ref _person, value); }
		}
	}
}
