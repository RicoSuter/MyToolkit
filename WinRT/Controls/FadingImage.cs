using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Animations;
using MyToolkit.Media;
using MyToolkit.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	public sealed class FadingImage : Control
	{
		public FadingImage()
		{
			DefaultStyleKey = typeof(FadingImage);
		}

		private Grid canvas;
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			canvas = (Grid)GetTemplateChild("canvas");
			UpdateSource();
		}


		public static readonly DependencyProperty CrossFadeImagesProperty =
			DependencyProperty.Register("CrossFadeImages", typeof (bool), typeof (FadingImage), new PropertyMetadata(false));

		public bool CrossFadeImages
		{
			get { return (bool) GetValue(CrossFadeImagesProperty); }
			set { SetValue(CrossFadeImagesProperty, value); }
		}




		public static readonly DependencyProperty FadingDurationProperty =
			DependencyProperty.Register("FadingDuration", typeof (TimeSpan), typeof (FadingImage), new PropertyMetadata(new TimeSpan(0, 0, 2)));

		public TimeSpan FadingDuration
		{
			get { return (TimeSpan) GetValue(FadingDurationProperty); }
			set { SetValue(FadingDurationProperty, value); }
		}





		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof (Uri), typeof (FadingImage), new PropertyMetadata(default(Uri), OnSourceChanged));

		private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var ctrl = (FadingImage) obj;
			ctrl.UpdateSource();
		}

		public Uri Source
		{
			get { return (Uri) GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}






		private Uri lastSource = null; 
		private bool animating = false; 
		async private void UpdateSource()
		{
			if (canvas == null || animating || lastSource == Source) // is animating or no change
				return; 

			if (lastSource == null)
			{
				SingleEvent.Register(ForegroundImage, 
					(image, handler) => image.ImageOpened += handler, 
					(image, handler) => image.ImageOpened -= handler, 
					async (o, args) =>
					{
						await Fading.FadeInAsync(ForegroundImage, FadingDuration);
						animating = false;
						UpdateSource();
					});

				SingleEvent.Register(ForegroundImage,
					(image, handler) => image.ImageFailed += handler,
					(image, handler) => image.ImageFailed -= handler,
					async (o, args) =>
					{
						animating = false;
						UpdateSource();
					});

				animating = true;
				ForegroundImage.Opacity = 0.0;
				ImageHelper.SetSource(ForegroundImage, Source);
			}
			else if (Source != null) // exchange images
			{
				animating = true;

				if (!CrossFadeImages)
					Fading.FadeOutAsync(ForegroundImage, FadingDuration);

				SingleEvent.Register(BackgroundImage,
					(image, handler) => image.ImageOpened += handler,
					(image, handler) => image.ImageOpened -= handler,
					async (o, args) =>
					{
						if (CrossFadeImages)
							await Task.WhenAll(new[]
							{
								Fading.FadeInAsync(BackgroundImage, FadingDuration),
								Fading.FadeOutAsync(ForegroundImage, FadingDuration)
							});
						else
							await Fading.FadeInAsync(BackgroundImage, FadingDuration);

						ImageHelper.SetSource(ForegroundImage, null);

						// reverse image order
						var fore = ForegroundImage;
						var back = BackgroundImage;
						canvas.Children.Clear();
						canvas.Children.Add(fore);
						canvas.Children.Add(back);

						animating = false;
						UpdateSource();
					});

				SingleEvent.Register(BackgroundImage,
					(image, handler) => image.ImageFailed += handler,
					(image, handler) => image.ImageFailed -= handler,
					async (o, args) =>
					{
						await Fading.FadeOutAsync(ForegroundImage, FadingDuration);
						animating = false;
						UpdateSource();
					});

				animating = true;
				BackgroundImage.Opacity = 0.0;
				ImageHelper.SetSource(BackgroundImage, Source);
			}
			else
			{
				animating = true;
				BackgroundImage.Opacity = 0.0;
				await Fading.FadeOutAsync(ForegroundImage, FadingDuration);
				animating = false;
				UpdateSource();
			}

			lastSource = Source; 
		}

		private Image ForegroundImage
		{
			get { return (Image)canvas.Children.Last(); }
		}

		private Image BackgroundImage
		{
			get { return (Image)canvas.Children.First(); }
		}
	}
}
