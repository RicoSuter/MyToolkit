using SamplePhoneApp.Domain;

namespace SamplePhoneApp.ViewModels
{
	public class DetailsPageViewModel : BaseViewModel<DetailsPageViewModel>
	{
		private Person person;
		public Person Person
		{
			get { return person; }
			set { SetProperty(m => m.Person, ref person, value); }
		}
	}
}
