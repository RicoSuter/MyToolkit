using MyToolkit.Collections;
using SamplePhoneApp.Domain;

namespace SamplePhoneApp.ViewModels
{
	public class ListPageViewModel: BaseViewModel<DetailsPageViewModel>
	{
		public ListPageViewModel()
		{
			Persons = new ObservableView<Person>(App.Persons.Collection, null, p => p.Name);
		}

		public ObservableView<Person> Persons { get; private set; }
	}
}
