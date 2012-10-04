using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MyToolkit.Messaging;
using MyToolkit.UI;
using SamplePhoneApp.Domain;
using SamplePhoneApp.ViewModels;

namespace SamplePhoneApp.Views
{
	public partial class DetailsPage : PhoneApplicationPage
	{
		public bool IsNewPage { get; private set; }
		public DetailsPageViewModel Model { get { return (DetailsPageViewModel) Resources["viewModel"]; } }

		public DetailsPage()
		{
			InitializeComponent();
			IsNewPage = true;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (IsNewPage)
			{
				Model.Person = this.ReadState<Person>("Person");
				if (Model.Person == null)
				{
					if (NavigationContext.QueryString.ContainsKey("SelectedItem"))
						Model.Person = App.Persons.Get(NavigationContext.QueryString["SelectedItem"]);
					else
						Model.Person = new Person();
				}
				IsNewPage = false;
			}
		}

		protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
		{
			TextMessage msg = null;
			if (!Model.Person.IsValid)
				msg = new TextMessage("Not valid", "Discard?", TextMessage.MessageButton.OKCancel); // TODO use strings from resource file

			if (msg != null)
			{
				Messenger.Send(msg);
				if (msg.Result == TextMessage.MessageResult.OK)
					App.Persons.Remove(Model.Person);
				else
					e.Cancel = true;
			}
			else
				App.Persons.Add(Model.Person);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			this.SaveState("Person", Model.Person);
		}
	}
}