using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	public class ImageButton : Button
	{
		public ImageButton()
		{
			DefaultStyleKey = typeof(ImageButton);
			IsEnabledChanged += OnIsEnabledChanged;
		}

		private void OnIsEnabledChanged(object sender, Windows.UI.Xaml.DependencyPropertyChangedEventArgs e)
		{
			Opacity = IsEnabled ? 1.0 : 0.35;
		}

		public static readonly DependencyProperty ImageProperty =
			DependencyProperty.Register("Image", typeof(ImageSource).FullName, typeof(ImageButton).FullName, null);

		public ImageSource Image
		{
			get { return (ImageSource)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}

		public static readonly DependencyProperty PressedImageProperty =
			DependencyProperty.Register("PressedImage", typeof(ImageSource).FullName, typeof(ImageButton).FullName, null);

		public ImageSource PressedImage
		{
			get { return (ImageSource)GetValue(PressedImageProperty); }
			set { SetValue(PressedImageProperty, value); }
		}
	}
}
