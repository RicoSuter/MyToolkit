using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using MyToolkit.Environment;
using MyToolkit.Multimedia;
using MyToolkit.Networking;
using MyToolkit.Paging;
using MyToolkit.Utilities;

namespace MyToolkit.Controls
{
	public partial class VideoButton : UserControl
	{
		public VideoButton()
		{
			InitializeComponent();

			SizeChanged += OnSizeChanged;
			Button.Click += OnClicked;

			PlayImage.Source = new BitmapImage(new Uri(PhoneApplication.IsDarkTheme ? "../Images/PlayIcon.png" : "../Images/PlayIconLight.png", UriKind.Relative));
		}

		public static readonly DependencyProperty ShowPlayIconProperty =
			DependencyProperty.Register("ShowPlayIcon", typeof (bool), typeof (VideoButton), new PropertyMetadata(true, ShowPlayIconPropertyChangedCallback));

		private static void ShowPlayIconPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var ctrl = (VideoButton)obj;
			ctrl.PlayImage.Visibility = ctrl.ShowPlayIcon ? Visibility.Visible : Visibility.Collapsed;
		}

		public bool ShowPlayIcon
		{
			get { return (bool) GetValue(ShowPlayIconProperty); }
			set { SetValue(ShowPlayIconProperty, value); }
		}

		public static readonly DependencyProperty ThumbnailSourceProperty =
			DependencyProperty.Register("ThumbnailSource", typeof(Uri), typeof(VideoButton), new PropertyMetadata(default(Uri), ThumbnailSourcePropertyChangedCallback));

		public Uri ThumbnailSource
		{
			get { return (Uri) GetValue(ThumbnailSourceProperty); }
			set { SetValue(ThumbnailSourceProperty, value); }
		}

		private static void ThumbnailSourcePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var ctrl = (VideoButton)dependencyObject;
			ctrl.UpdateImage();
		}

		public static readonly DependencyProperty VideoSourceProperty =
			DependencyProperty.Register("VideoSource", typeof (Uri), typeof (VideoButton), new PropertyMetadata(default(Uri)));

		public Uri VideoSource
		{
			get { return (Uri) GetValue(VideoSourceProperty); }
			set { SetValue(VideoSourceProperty, value); }
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
				Image.Height = (ActualWidth / 480) * 270;

				Button.Width = ActualWidth;
				Button.Height = (ActualWidth / 480) * 270; 

				Media.ImageHelper.SetSource(Image, ThumbnailSource);
			}
		}

		private void OnClicked(object sender, RoutedEventArgs args)
		{
			if (VideoSource != null && NavigationState.TryBeginNavigating())
			{
				var player = new MediaPlayerLauncher
				{
					Controls = MediaPlaybackControls.All,
					Media = VideoSource
				};
				player.Show();
			}
		}
	}
}
