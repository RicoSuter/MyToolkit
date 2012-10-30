using System;
using MyToolkit.Utilities;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endif

namespace MyToolkit.Controls
{
	public class PanAndZoomImage : PanAndZoomViewer
	{
		public event EventHandler ImageLoaded;

		private Image image;
		public PanAndZoomImage()
		{
			DefaultStyleKey = typeof(PanAndZoomImage);
		}

		public event EventHandler<RoutedEventArgs> ImageOpened
		{
			add { image.ImageOpened += value; }
			remove { image.ImageOpened += value; }
		}

		public event EventHandler<ExceptionRoutedEventArgs> ImageFailed
		{
			add { image.ImageFailed += value; }
			remove { image.ImageFailed += value; }
		}

#if WINRT
		protected override void OnApplyTemplate()
#else
		public override void OnApplyTemplate()
#endif
		{
			base.OnApplyTemplate();

			image = (Image)GetTemplateChild("image");
			image.IsHitTestVisible = false;


			DependencyPropertyChangedEvent.Register(image, Image.SourceProperty, OnSourcePropertyChanged);
			SizeChanged += delegate { UpdateMaxZoomFactor(); };
		}

		private void OnSourcePropertyChanged(object arg1, object arg2)
		{
			UpdateMaxZoomFactor();

			var copy = ImageLoaded;
			if (copy != null)
				copy(this, new EventArgs());
		}
		
		private void UpdateMaxZoomFactor()
		{
			if (AutomaticZoomFactor && ActualHeight > 0 && ActualWidth > 0)
			{
				var bitmap = image.Source as BitmapImage;
				if (bitmap != null)
					CalculateMaxZoomFactor(bitmap.PixelWidth, bitmap.PixelHeight);
			}
		}

		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(Uri), typeof(PanAndZoomImage), new PropertyMetadata(default(Uri)));

		public Uri Source
		{
			get { return (Uri)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public static readonly DependencyProperty StretchProperty =
			DependencyProperty.Register("Stretch", typeof(Stretch), typeof(PanAndZoomImage), new PropertyMetadata(default(Stretch)));

		public Stretch Stretch
		{
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}

		public static readonly DependencyProperty AutomaticMaxZoomFactorProperty =
			DependencyProperty.Register("AutomaticMaxZoomFactor", typeof (bool), typeof (PanAndZoomImage), new PropertyMetadata(true));

		public bool AutomaticZoomFactor
		{
			get { return (bool) GetValue(AutomaticMaxZoomFactorProperty); }
			set { SetValue(AutomaticMaxZoomFactorProperty, value); }
		}

		public static readonly DependencyProperty InnerMarginProperty =
			DependencyProperty.Register("InnerMargin", typeof (Thickness), typeof (PanAndZoomImage), new PropertyMetadata(default(Thickness)));

		public Thickness InnerMargin
		{
			get { return (Thickness) GetValue(InnerMarginProperty); }
			set { SetValue(InnerMarginProperty, value); }
		}
	}
}