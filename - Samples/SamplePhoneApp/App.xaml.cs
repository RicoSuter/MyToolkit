using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyToolkit.Messaging;
using MyToolkit.Utilities;
using SamplePhoneApp.Domain;

namespace SamplePhoneApp
{
	public partial class App : Application
	{
		public PhoneApplicationFrame RootFrame { get; private set; }
		public App()
		{
			UnhandledException += Application_UnhandledException;
			InitializeComponent();
			InitializePhoneApplication();
			if (System.Diagnostics.Debugger.IsAttached)
				Current.Host.Settings.EnableFrameRateCounter = true;

			Persons = new EntityContainer<Person>();
		}

		#region Entitites

		public static EntityContainer<Person> Persons { get; private set; }

		#endregion

		#region Lifecycle

		private void LoadData()
		{
			Messenger.Register<TextMessage>(DefaultActions.ShowTextMessage);
			Persons.LoadFromSettings("Persons");
		}

		private void SaveData()
		{
			Messenger.Unregister();
			Persons.SaveToSettings("Persons");
		}

		#endregion

		#region Lifecycle events

		private void Application_Launching(object sender, LaunchingEventArgs e)
		{
			LoadData();
		}

		private void Application_Activated(object sender, ActivatedEventArgs e)
		{
			LoadData();
		}

		private void Application_Deactivated(object sender, DeactivatedEventArgs e)
		{
			SaveData();
		}

		private void Application_Closing(object sender, ClosingEventArgs e)
		{
			SaveData();
		}

		#endregion

		#region Exceptions

		private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
				System.Diagnostics.Debugger.Break();
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
				System.Diagnostics.Debugger.Break();
		}

		#endregion

		#region Phone application initialization

		private bool phoneApplicationInitialized = false;
		private void InitializePhoneApplication()
		{
			if (phoneApplicationInitialized)
				return;

			// RootFrame = new PhoneApplicationFrame();
			RootFrame = new TransitionFrame(); 
			RootFrame.Navigated += CompleteInitializePhoneApplication;

			RootFrame.NavigationFailed += RootFrame_NavigationFailed;
			phoneApplicationInitialized = true;
		}

		private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
		{
			if (RootVisual != RootFrame)
				RootVisual = RootFrame;
			RootFrame.Navigated -= CompleteInitializePhoneApplication;
		}

		#endregion
	}
}