using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace MyToolkit.UI
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
