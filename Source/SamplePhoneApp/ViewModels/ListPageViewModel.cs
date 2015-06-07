using MyToolkit.Collections;
using SamplePhoneApp.Domain;

namespace SamplePhoneApp.ViewModels
{
	public class ListPageViewModel: ViewModelBase
	{
		public ListPageViewModel()
		{
			Persons = new ObservableCollectionView<Person>(App.Persons.Collection, null, p => p.Name);
		}

        public ObservableCollectionView<Person> Persons { get; private set; }
	}
}
