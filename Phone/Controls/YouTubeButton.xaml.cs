using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MyToolkit.Environment;
using MyToolkit.Multimedia;
using MyToolkit.Networking;

namespace MyToolkit.Controls
{
	/// <summary>
	/// Add YouTube play logic to Click event (see http://mytoolkit.codeplex.com/wikipage?title=YouTube)
	/// </summary>
	public partial class YouTubeButton : UserControl
	{
		public YouTubeButton()
		{
			InitializeComponent();

			SizeChanged += OnSizeChanged;
			Button.Click += (o, e) => { if (Click != null) Click(this, e); };

			PlayImage.Source = new BitmapImage(new Uri(PhoneApplication.IsDarkTheme ? "../Images/PlayIcon.png" : "../Images/PlayIconLight.png", UriKind.Relative));
		}

		public static readonly DependencyProperty ShowPlayIconProperty =
			DependencyProperty.Register("ShowPlayIcon", typeof (bool), typeof (YouTubeButton), new PropertyMetadata(true, ShowPlayIconPropertyChangedCallback));

		private static void ShowPlayIconPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var ctrl = (YouTubeButton)obj;
			ctrl.PlayImage.Visibility = ctrl.ShowPlayIcon ? Visibility.Visible : Visibility.Collapsed;
		}

		public bool ShowPlayIcon
		{
			get { return (bool) GetValue(ShowPlayIconProperty); }
			set { SetValue(ShowPlayIconProperty, value); }
		}

		public static readonly DependencyProperty YouTubeIDProperty =
			DependencyProperty.Register("YouTubeID", typeof (string), typeof (YouTubeButton), new PropertyMetadata(default(string), PropertyChangedCallback));

		public string YouTubeID
		{
			get { return (string) GetValue(YouTubeIDProperty); }
			set { SetValue(YouTubeIDProperty, value); }
		}

		public event RoutedEventHandler Click;

		private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var ctrl = (YouTubeButton)dependencyObject;
			ctrl.LoadUri();
		}

		private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
		{
			UpdateImage();
		}

		private void UpdateImage()
		{
			if (ActualWidth > 0.0)
			{
				Image.Width = ActualWidth;
				Image.Height = (ActualWidth / 480) * 360;

				Button.Width = ActualWidth;
				Button.Height = (ActualWidth / 480) * 360; 

				Performance.LowProfileImageLoader.SetUriSource(Image, imageUri);
			}
		}

		private Uri imageUri; 
		private void LoadUri()
		{
			if (!String.IsNullOrEmpty(YouTubeID))
			{
				imageUri = YouTube.GetThumbnailUri(YouTubeID);
				UpdateImage();
			}
			else
				imageUri = null; 
		}
	}
}
