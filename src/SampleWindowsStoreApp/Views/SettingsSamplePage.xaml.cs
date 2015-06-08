using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using MyToolkit.Paging;
using MyToolkit.UI;
using SampleWindowsStoreApp.Views.Settings;

namespace SampleWindowsStoreApp.Views
{
    public sealed partial class SettingsSamplePage
    {
        // Go to the ApplicationSettings class to actually see how to read and write settings

        // See http://mytoolkit.codeplex.com/wikipage?title=ApplicationSettings

        public SettingsSamplePage()
        {
            InitializeComponent();
        }

        // TODO: This should be implemented in Startup() of App.xaml.cs

        protected override void OnNavigatedTo(MtNavigationEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += OnSettingsCommandsRequested;
		}

        protected override void OnNavigatingFrom(MtNavigatingCancelEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= OnSettingsCommandsRequested;
        }
        
        private void OnSettingsCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
		{
			var cmd = new SettingsCommand("settings", "Settings", ShowSettings);
			args.Request.ApplicationCommands.Add(cmd);

			cmd = new SettingsCommand("privacy", "Privacy Policy", ShowPrivacyPolicy);
			args.Request.ApplicationCommands.Add(cmd);

			cmd = new SettingsCommand("help", "Help", ShowHelp);
			args.Request.ApplicationCommands.Add(cmd);
		}

        // For information about the SettingsFlyout class go to http://msdn.microsoft.com/en-us/library/windows/apps/hh872190.aspx
        
        private void ShowSettings(IUICommand command)
        {
            new MySettingsFlyout().Show();
        }

        private void ShowPrivacyPolicy(IUICommand command)
        {
            var flyout = new SettingsFlyout();
            flyout.Title = "Privacy Policy";
            flyout.Show();
        }

        // TODO: Implement own settings views (UserControl based on SettingsFlyout) 

        private void ShowHelp(IUICommand command)
        {
            var flyout = new SettingsFlyout();
            flyout.Title = "Help";
            flyout.Show();
        }
    }
}
