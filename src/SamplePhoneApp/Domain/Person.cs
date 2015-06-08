using System.Linq;
using System.Runtime.Serialization;
using MyToolkit.Data;
using MyToolkit.Model;
using MyToolkit.MVVM;
using MyToolkit.Utilities;

namespace SamplePhoneApp.Domain
{
	public class Person : ObservableObject, IEntity<string>
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
		public string Id { get; set; }

		private string _firstName;
		[DataMember]
		public string FirstName
		{
			get { return _firstName; }
			set { SetProperty(ref _firstName, value); }
		}

		private string _lastName;
		[DataMember]
		public string LastName
		{
			get { return _lastName; }
			set { Set(ref _lastName, value); }
		}

		public string Name
		{
			get { return FirstName + " " + LastName; }
		}
	}
}
