using System.Collections.ObjectModel;
using MyToolkit.MVVM;
using MyToolkit.Mvvm;
using SampleWindowsStoreApp.Domain;

namespace SampleWindowsStoreApp.ViewModels
{
    public class DataGridPageModel : ViewModelBase
	{
        private Person _selectedPerson;

        public ObservableCollection<Person> People { get; private set; }

        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set { Set(ref _selectedPerson, value); }
        }

        public DataGridPageModel()
		{
			People = new ObservableCollection<Person>
			{
				new Person { Firstname = "Rico", Lastname = "Suter", Category = "A"},
				new Person { Firstname = "John", Lastname = "Doe", Category = "B"},
				new Person { Firstname = "Max", Lastname = "Muster", Category = "C"},
			};

            SelectedPerson = People[1];
		}
	}
}
