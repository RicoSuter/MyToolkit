using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MyToolkit.Environment;

namespace MyToolkit.Controls
{
	public partial class ExtendedImage : UserControl
	{
		public ExtendedImage()
		{
			InitializeComponent();
		}

		#region Properties

		public static readonly DependencyProperty ShowBackgroundProperty =
			DependencyProperty.Register("ShowBackground", typeof (bool), typeof (ExtendedImage), new PropertyMetadata(true, PropertyChangedCallback));

		public bool ShowBackground
		{
			get { return (bool) GetValue(ShowBackgroundProperty); }
			set { SetValue(ShowBackgroundProperty, value); }
		}

		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(Uri), typeof(ExtendedImage), new PropertyMetadata(default(Uri), PropertyChangedCallback));

		public Uri Source
		{
			get { return (Uri) GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public static readonly DependencyProperty SourceLightProperty =
			DependencyProperty.Register("SourceLight", typeof (Uri), typeof (ExtendedImage), new PropertyMetadata(default(Uri), PropertyChangedCallback));

		public Uri SourceLight
		{
			get { return (Uri) GetValue(SourceLightProperty); }
			set { SetValue(SourceLightProperty, value); }
		}

		public static readonly DependencyProperty StretchProperty =
			DependencyProperty.Register("Stretch", typeof (Stretch), typeof (ExtendedImage), new PropertyMetadata(default(Stretch), PropertyChangedCallback));

		public Stretch Stretch
		{
			get { return (Stretch) GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}

		#endregion

		private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var ctrl = (ExtendedImage)obj;
			if (e.Property == StretchProperty)
				ctrl.Image.Stretch = ctrl.Stretch;
			else if (e.Property == ShowBackgroundProperty)
			{
				ctrl.LayoutRoot.Background = ctrl.ShowBackground ?
					(Brush)ctrl.Resources["PhoneInactiveBrush"] : new SolidColorBrush(Colors.Transparent);
			}
			else if (e.Property == SourceLightProperty || e.Property == SourceProperty)
				ctrl.UpdateSource();
		}

		private void UpdateSource()
		{
			if (PhoneApplication.IsDarkTheme || SourceLight == null)
				Media.ImageHelper.SetSource(Image, Source);
			else
				Media.ImageHelper.SetSource(Image, SourceLight);
		}
	}
}
