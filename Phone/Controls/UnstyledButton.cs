using System.Windows;
using System.Windows.Controls;
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

		public event RoutedEventHandler Click;
		public UnstyledButton()
		{
			DefaultStyleKey = typeof(UnstyledButton);
		}

		protected override void OnTap(System.Windows.Input.GestureEventArgs e)
		{
			base.OnTap(e);
			if (Click != null)
				Click(this, new RoutedEventArgs());
		}
	}
}
