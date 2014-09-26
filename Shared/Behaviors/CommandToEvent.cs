using System.Windows.Input;
using MyToolkit.Utilities;
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Behaviors
{
    public class CommandToEvent : Behavior<FrameworkElement>
    {
        private object _eventToken = null;
        private string _oldEvent = null; 

        public static readonly DependencyProperty EventProperty = DependencyProperty.Register(
            "Event", typeof(string), typeof(CommandToEvent), new PropertyMetadata(default(string), 
                (o, args) => ((CommandToEvent)o).UpdateEventRegistration()));

        public string Event
        {
            get { return (string) GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof (ICommand), typeof (CommandToEvent), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof (object), typeof (CommandToEvent), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object) GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void OnAttached()
        {
            UpdateEventRegistration();
        }

        protected override void OnDetaching()
        {
            DeregisterEventIfNeeded();
        }

        private void UpdateEventRegistration()
        {
            if (Element == null)
                return;

            DeregisterEventIfNeeded();
            if (!string.IsNullOrEmpty(Event))
            {
                _eventToken = EventUtilities.RegisterEvent(Element, Event, OnEventCalled);
                _oldEvent = Event;
            }
        }

        private void DeregisterEventIfNeeded()
        {
            if (_eventToken != null)
            {
                EventUtilities.DeregisterEvent(Element, _oldEvent, _eventToken);
                _eventToken = null;
            }
        }

        private void OnEventCalled(object sender, object args)
        {
            var command = Command;
            var parameter = CommandParameter;

            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}
