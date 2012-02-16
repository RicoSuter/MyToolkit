using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyToolkit.Network;

namespace MyToolkit.Phone
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
		}

		public static readonly DependencyProperty UseWidescreenProperty =
			DependencyProperty.Register("UseWidescreen", typeof (bool), typeof (YouTubeButton), new PropertyMetadata(default(bool)));

		// TODO: does not work yet!
		public bool UseWidescreen
		{
			get { return (bool) GetValue(UseWidescreenProperty); }
			set { SetValue(UseWidescreenProperty, value); }
		}

		public static readonly DependencyProperty YouTubeIDProperty =
			DependencyProperty.Register("YouTubeID", typeof (string), typeof (YouTubeButton), new PropertyMetadata(default(string), PropertyChangedCallback));

		public string YouTubeID
		{
			get { return (string) GetValue(YouTubeIDProperty); }
			set { SetValue(YouTubeIDProperty, value); }
		}

		public event RoutedEventHandler Click
		{
			add { Button.Click += value; }
			remove { Button.Click -= value; }
		}

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
				Image.Height = (ActualWidth / 480) * (UseWidescreen ? 270 : 360);

				Button.Width = ActualWidth;
				Button.Height = (ActualWidth / 480) * (UseWidescreen ? 270 : 360);

				Performance.LowProfileImageLoader.SetUriSource(Image, imageUri);
			}
		}

		private Uri imageUri; 
		private void LoadUri()
		{
			if (!String.IsNullOrEmpty(YouTubeID))
				YouTube.GetThumnailUrl(YouTubeID, ThumbnailReceived);
			else
				imageUri = null; 
		}

		private void ThumbnailReceived(HttpResponse response, Uri uri)
		{
			imageUri = uri; 
			Dispatcher.BeginInvoke(UpdateImage);
		}
	}
}
