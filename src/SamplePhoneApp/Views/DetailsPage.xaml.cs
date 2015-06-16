using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MyToolkit.Messaging;
using MyToolkit.UI;
using SamplePhoneApp.Domain;
using SamplePhoneApp.ViewModels;

namespace SamplePhoneApp.Views
{
	public partial class DetailsPage 
	{
		public DetailsPageViewModel Model { get { return (DetailsPageViewModel) Resources["viewModel"]; } }

		public DetailsPage()
		{
			InitializeComponent();
			AddBackKeyPressHandler(BackKeyPressHandler);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e, bool isNewPage)
		{
			if (isNewPage)
			{
				Model.Person = this.ReadState<Person>("Person");
				if (Model.Person == null)
				{
					if (NavigationContext.QueryString.ContainsKey("SelectedItem"))
						Model.Person = App.Persons.Get(NavigationContext.QueryString["SelectedItem"]);
					else
						Model.Person = new Person();
				}
			}
		}

		protected void BackKeyPressHandler(CancelEventArgs e)
		{
            if (!Model.Person.IsValid)
			{
                var msg = new TextMessage("Not valid", "Discard?", MessageButton.OKCancel); // TODO use strings from resource file
                msg.SuccessCallback += result =>
			    {
			        if (result == MessageResult.Ok)
			        {
			            App.Persons.Remove(Model.Person);
			            NavigationService.GoBack();
			        }
			    };

                e.Cancel = true;
                Messenger.Default.Send(msg);
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