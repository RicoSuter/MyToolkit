using System.Linq;
using System.Runtime.Serialization;
using MyToolkit.MVVM;
using MyToolkit.Utilities;

namespace SamplePhoneApp.Domain
{
	public class Person : NotifyPropertyChanged<Person>, IEntity
	{
		public Person()
		{
			Id = App.Persons.GenerateIdentity();
		}

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(FirstName) && 
					!string.IsNullOrEmpty(LastName) && 
					!App.Persons.Collection.Any(s => s != this && s.Name == Name);
			}
		}

		[DataMember]
		public int Id { get; set; }

		private string firstName;
		[DataMember]
		public string FirstName
		{
			get { return firstName; }
			set { SetProperty(m => m.FirstName, ref firstName, value); }
		}

		private string lastName;
		[DataMember]
		public string LastName
		{
			get { return lastName; }
			set { SetProperty(m => m.LastName, ref lastName, value); }
		}

		public string Name
		{
			get { return FirstName + " " + LastName; }
		}
	}
}
