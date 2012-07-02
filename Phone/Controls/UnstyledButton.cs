using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;

namespace MyToolkit.Controls
{
	public class UnstyledButton : ContentControl
	{
		static UnstyledButton()
		{
			if (!TiltEffect.TiltableItems.Contains(typeof(UnstyledButton)))
				TiltEffect.TiltableItems.Add(typeof(UnstyledButton));
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof (ICommand), typeof (UnstyledButton), new PropertyMetadata(default(ICommand)));

		public ICommand Command
		{
			get { return (ICommand) GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public event RoutedEventHandler Click;
		public UnstyledButton()
		{
			DefaultStyleKey = typeof(UnstyledButton);
		}

		protected override void OnTap(System.Windows.Input.GestureEventArgs e)
		{
			base.OnTap(e);

			var copy = Click;
			if (copy != null)
				copy(this, new RoutedEventArgs());

			if (Command != null && Command.CanExecute(null))
				Command.Execute(null);
		}
	}
}
