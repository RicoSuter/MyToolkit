using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MyToolkit.Utilities;

namespace MyToolkit.Controls
{
	public class PanAndZoomImage : Control
	{
		public PanAndZoomImage()
		{
			DefaultStyleKey = typeof(PanAndZoomImage);
		}

		private Image image;
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			image = (Image)GetTemplateChild("image");
			image.IsHitTestVisible = false;

			DependencyPropertyChangedEvent.Register(image, Image.SourceProperty, UpdateMaxZoomFactor);
			SizeChanged += delegate { UpdateMaxZoomFactor(null, null); };
		}
		
		private void UpdateMaxZoomFactor(object sender, object value)
		{
			if (AutomaticZoomFactor && ActualHeight > 0 && ActualWidth > 0)
			{
				var bitmap = image.Source as BitmapImage;
				if (bitmap != null)
				{
					var horizontalZoom = bitmap.PixelWidth / ActualWidth;
					var verticalZoom = bitmap.PixelHeight / ActualHeight;

					MaxZoomFactor = horizontalZoom > verticalZoom ? horizontalZoom : verticalZoom;
				}
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

		public static readonly DependencyProperty MaxZoomFactorProperty =
			DependencyProperty.Register("MaxZoomFactor", typeof (double), typeof (PanAndZoomImage), new PropertyMetadata(5.0));

		public double MaxZoomFactor
		{
			get { return (double) GetValue(MaxZoomFactorProperty); }
			set { SetValue(MaxZoomFactorProperty, value); }
		}

		public static readonly DependencyProperty AutomaticMaxZoomFactorProperty =
			DependencyProperty.Register("AutomaticMaxZoomFactor", typeof (bool), typeof (PanAndZoomImage), new PropertyMetadata(true));

		public bool AutomaticZoomFactor
		{
			get { return (bool) GetValue(AutomaticMaxZoomFactorProperty); }
			set { SetValue(AutomaticMaxZoomFactorProperty, value); }
		}	
	}
}