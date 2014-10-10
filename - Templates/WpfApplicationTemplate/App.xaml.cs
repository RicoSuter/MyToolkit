using System.Windows;
using MyToolkit.Messaging;

namespace WpfApplicationTemplate
{
    /// <summary>Interaction logic for App.xaml</summary>
    public partial class App : Application
    {
        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup"/> event. </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs"/> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // TODO: Initialize application and register more message types if needed. 
            Messenger.Default.Register(DefaultActions.GetTextMessageAction());
            
            base.OnStartup(e);
        }
    }
}
