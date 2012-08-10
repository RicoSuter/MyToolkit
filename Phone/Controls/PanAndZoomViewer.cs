using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace MyToolkit.Controls
{
	[ContentProperty("Content")]
	public class PanAndZoomViewer : Control
	{
		public PanAndZoomViewer()
		{
			DefaultStyleKey = typeof(PanAndZoomViewer);
		}

		private ContentPresenter content; 
		private RectangleGeometry geometry; 
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			content = (ContentPresenter)GetTemplateChild("content");
			content.Content = Content; 
			
			geometry = new RectangleGeometry();
			Clip = geometry;

			SizeChanged += delegate { UpdateGeometry(); };
			UpdateGeometry();

			var listener = GestureService.GetGestureListener(this);
			listener.PinchStarted += OnPinchStarted;
			listener.PinchDelta += OnPinchDelta;
			listener.DragDelta += OnDragDelta;
		}

		protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
		{
			e.Handled = true;
			base.OnManipulationDelta(e);
		}
		protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
		{
			e.Handled = true;
			base.OnManipulationStarted(e);
		}

		protected override void OnDoubleTap(System.Windows.Input.GestureEventArgs e)
		{
			e.Handled = true;

			// TODO animate
			
			if (TotalImageScale == 1 && ImagePosition.X == 0 && ImagePosition.Y == 0)
			{
				var point = e.GetPosition(this);
				TotalImageScale = MaxZoomFactor;

				var width = point.X * MaxZoomFactor;
				if (width > content.ActualWidth * MaxZoomFactor - ActualWidth / 2)
					width = content.ActualWidth * MaxZoomFactor - ActualWidth / 2;
				if (width < ActualHeight / 2)
					width = ActualWidth / 2;

				var height = point.Y * MaxZoomFactor;
				if (height > content.ActualHeight * MaxZoomFactor - ActualHeight / 2)
					height = content.ActualHeight * MaxZoomFactor - ActualHeight / 2;
				if (height < ActualHeight / 2)
					height = ActualHeight / 2;

				ImagePosition = new Point((width - ActualWidth / 2) * -1, (height - ActualHeight / 2) * -1);

				ApplyScale();
				ApplyPosition();
			}
			else
				ResetZoomAndPosition();
	
			base.OnDoubleTap(e);
		}

		private void UpdateGeometry()
		{
			geometry.Rect = new Rect(0, 0, ActualWidth, ActualHeight);
		}
		
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof (FrameworkElement), typeof (PanAndZoomViewer), new PropertyMetadata(default(FrameworkElement)));

		public FrameworkElement Content
		{
			get { return (FrameworkElement) GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}

		public static readonly DependencyProperty MaxZoomFactorProperty =
			DependencyProperty.Register("MaxZoomFactor", typeof (double), typeof (PanAndZoomViewer), new PropertyMetadata(5.0));

		public double MaxZoomFactor
		{
			get { return (double) GetValue(MaxZoomFactorProperty); }
			set { SetValue(MaxZoomFactorProperty, value); }
		}
		





		// these two fields fully define the zoom state:
		private double TotalImageScale = 1d;
		private Point ImagePosition = new Point(0, 0);


		private Point _oldFinger1;
		private Point _oldFinger2;
		private double _oldScaleFactor;


		#region Event handlers

		/// <summary>
		/// Initializes the zooming operation
		/// </summary>
		private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
		{
			_oldFinger1 = e.GetPosition(content, 0);
			_oldFinger2 = e.GetPosition(content, 1);
			_oldScaleFactor = 1;
			e.Handled = true; 
		}

		/// <summary>
		/// Computes the scaling and translation to correctly zoom around your fingers.
		/// </summary>
		private void OnPinchDelta(object sender, PinchGestureEventArgs e)
		{
			var scaleFactor = e.DistanceRatio / _oldScaleFactor;
			if (!IsScaleValid(scaleFactor))
				return;

			var currentFinger1 = e.GetPosition(content, 0);
			var currentFinger2 = e.GetPosition(content, 1);

			var translationDelta = GetTranslationDelta(
				currentFinger1,
				currentFinger2,
				_oldFinger1,
				_oldFinger2,
				ImagePosition,
				scaleFactor);

			_oldFinger1 = currentFinger1;
			_oldFinger2 = currentFinger2;
			_oldScaleFactor = e.DistanceRatio;

			UpdateImageScale(scaleFactor);
			UpdateImagePosition(translationDelta);

			e.Handled = true; 
		}

		/// <summary>
		/// Moves the image around following your finger.
		/// </summary>
		private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
		{
			var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);

			if (IsDragValid(1, translationDelta))
				UpdateImagePosition(translationDelta);

			e.Handled = true; 
		}

		#endregion

		#region Utils

		/// <summary>
		/// Computes the translation needed to keep the image centered between your fingers.
		/// </summary>
		private Point GetTranslationDelta(
			Point currentFinger1, Point currentFinger2,
			Point oldFinger1, Point oldFinger2,
			Point currentPosition, double scaleFactor)
		{
			var newPos1 = new Point(
			 currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
			 currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

			var newPos2 = new Point(
			 currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
			 currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

			var newPos = new Point(
				(newPos1.X + newPos2.X) / 2,
				(newPos1.Y + newPos2.Y) / 2);

			return new Point(
				newPos.X - currentPosition.X,
				newPos.Y - currentPosition.Y);
		}

		/// <summary>
		/// Updates the scaling factor by multiplying the delta.
		/// </summary>
		private void UpdateImageScale(double scaleFactor)
		{
			TotalImageScale *= scaleFactor;
			ApplyScale();
		}

		/// <summary>
		/// Applies the computed scale to the image control.
		/// </summary>
		private void ApplyScale()
		{
			((CompositeTransform)content.RenderTransform).ScaleX = TotalImageScale;
			((CompositeTransform)content.RenderTransform).ScaleY = TotalImageScale;
		}

		/// <summary>
		/// Updates the image position by applying the delta.
		/// Checks that the image does not leave empty space around its edges.
		/// </summary>
		private void UpdateImagePosition(Point delta)
		{
			var newPosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

			if (newPosition.X > 0) newPosition.X = 0;
			if (newPosition.Y > 0) newPosition.Y = 0;

			if ((content.ActualWidth * TotalImageScale) + newPosition.X < content.ActualWidth)
				newPosition.X = content.ActualWidth - (content.ActualWidth * TotalImageScale);

			if ((content.ActualHeight * TotalImageScale) + newPosition.Y < content.ActualHeight)
				newPosition.Y = content.ActualHeight - (content.ActualHeight * TotalImageScale);

			ImagePosition = newPosition;

			ApplyPosition();
		}

		/// <summary>
		/// Applies the computed position to the image control.
		/// </summary>
		private void ApplyPosition()
		{
			((CompositeTransform)content.RenderTransform).TranslateX = ImagePosition.X;
			((CompositeTransform)content.RenderTransform).TranslateY = ImagePosition.Y;
		}

		public void ResetZoomAndPosition()
		{
			TotalImageScale = 1;
			ImagePosition = new Point(0, 0);

			ApplyScale();
			ApplyPosition();
		}

		/// <summary>
		/// Checks that dragging by the given amount won't result in empty space around the image
		/// </summary>
		private bool IsDragValid(double scaleDelta, Point translateDelta)
		{
			if (ImagePosition.X + translateDelta.X > 0 || ImagePosition.Y + translateDelta.Y > 0)
				return false;

			if ((content.ActualWidth * TotalImageScale * scaleDelta) + (ImagePosition.X + translateDelta.X) < content.ActualWidth)
				return false;

			if ((content.ActualHeight * TotalImageScale * scaleDelta) + (ImagePosition.Y + translateDelta.Y) < content.ActualHeight)
				return false;

			return true;
		}

		/// <summary>
		/// Tells if the scaling is inside the desired range
		/// </summary>
		private bool IsScaleValid(double scaleDelta)
		{
			return (TotalImageScale * scaleDelta >= 1) && (TotalImageScale * scaleDelta <= MaxZoomFactor);
		}

		#endregion
	}
}
