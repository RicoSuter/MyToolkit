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

namespace MyToolkit.UI
{
	public class ImageButton : Button
	{
		public ImageButton()
		{
			DefaultStyleKey = typeof(ImageButton);
			IsEnabledChanged += OnIsEnabledChanged;
		}

		private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Opacity = IsEnabled ? 1.0 : 0.5;
		}

		public static readonly DependencyProperty ImageProperty = 
			DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), null);

		public ImageSource Image
		{
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }

		}

		public static readonly DependencyProperty PressedImageProperty = 
			DependencyProperty.Register("PressedImage", typeof(ImageSource), typeof(ImageButton), null);

		public ImageSource PressedImage
		{
			get { return (ImageSource)GetValue(PressedImageProperty); }
			set { SetValue(PressedImageProperty, value); }
		}
	}
}
