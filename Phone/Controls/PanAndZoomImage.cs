using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyToolkit.Controls
{
	public class PanAndZoomImage : Control
	{
		public PanAndZoomImage()
		{
			DefaultStyleKey = typeof(PanAndZoomImage);
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
			DependencyProperty.Register("MaxZoomFactor", typeof (double), typeof (PanAndZoomImage), new PropertyMetadata(default(double)));

		public double MaxZoomFactor
		{
			get { return (double) GetValue(MaxZoomFactorProperty); }
			set { SetValue(MaxZoomFactorProperty, value); }
		}
	}
}