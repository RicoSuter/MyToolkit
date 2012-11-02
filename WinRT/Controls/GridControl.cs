using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MyToolkit.Controls
{
	public class GridControl : Canvas
	{
		public static readonly DependencyProperty OffsetXProperty =
			DependencyProperty.Register("OffsetX", typeof(double), typeof(GridControl), new PropertyMetadata(0.0,
				(o, args) => ((GridControl)o).UpdateOffsetX((double)args.OldValue, (double)args.NewValue)));

		public double OffsetX
		{
			get { return (double) GetValue(OffsetXProperty); }
			set { SetValue(OffsetXProperty, value); }
		}

		public static readonly DependencyProperty OffsetYProperty =
			DependencyProperty.Register("OffsetY", typeof (double), typeof (GridControl), new PropertyMetadata(0.0, 
				(o, args) => ((GridControl)o).UpdateOffsetY((double) args.OldValue, (double) args.NewValue)));

		public double OffsetY
		{
			get { return (double)GetValue(OffsetYProperty); }
			set { SetValue(OffsetYProperty, value); }
		}

		public static readonly DependencyProperty CellHeightProperty =
			DependencyProperty.Register("CellHeight", typeof(double), typeof(GridControl), new PropertyMetadata(5.0, PropertyChangedCallback));

		public double CellHeight
		{
			get { return (double)GetValue(CellHeightProperty); }
			set { SetValue(CellHeightProperty, value); }
		}

		public static readonly DependencyProperty CellWidthProperty =
			DependencyProperty.Register("CellWidth", typeof(double), typeof(GridControl), new PropertyMetadata(5.0, PropertyChangedCallback));

		public double CellWidth
		{
			get { return (double)GetValue(CellWidthProperty); }
			set { SetValue(CellWidthProperty, value); }
		}

		public static readonly DependencyProperty StrokeProperty =
			DependencyProperty.Register("Stroke", typeof(Brush), typeof(GridControl), new PropertyMetadata(new SolidColorBrush(Colors.White), PropertyChangedCallback));

		public Brush Stroke
		{
			get { return (Brush) GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}

		private static void PropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			((GridControl)o).Update();
		}

		public GridControl()
		{
			LayoutUpdated += delegate { Update(); };
		}

		private void UpdateOffsetX(double oldOffset, double newOffset)
		{
			var oldDiff = oldOffset % CellWidth;
			var diff = newOffset % CellWidth;
			foreach (var line in verticalLines)
			{
				line.X1 = line.X1 - oldDiff + diff;
				line.X2 = line.X2 - oldDiff + diff;
			}
		}

		private void UpdateOffsetY(double oldOffset, double newOffset)
		{
			var oldDiff = oldOffset % CellHeight;
			var diff = newOffset % CellHeight;
			foreach (var line in horizontalLines)
			{
				line.Y1 = line.Y1 - oldDiff + diff;
				line.Y2 = line.Y2 - oldDiff + diff;
			}
		}

		private double lastWidth = 0.0;
		private double lastHeight = 0.0;
		private List<Line> verticalLines = new List<Line>(); 
		private List<Line> horizontalLines = new List<Line>(); 

		public void Update()
		{
			if (ActualWidth == lastWidth && ActualHeight == lastHeight)
				return;

			lastWidth = ActualWidth;
			lastHeight = ActualHeight;

			Children.Clear();
			verticalLines.Clear();
			horizontalLines.Clear();

			for (var x = -CellWidth * 2; x < ActualWidth + CellWidth * 2; x = x + CellWidth) // vertical lines
			{
				var line = new Line();
				line.Stroke = Stroke;
				line.X1 = x + OffsetX;
				line.Y1 = -OffsetY * 2; 
				line.X2 = x + OffsetX;
				line.Y2 = ActualHeight + OffsetY * 2; 
				Children.Add(line);
				verticalLines.Add(line);
			}

			for (var y = -CellHeight * 2; y < ActualHeight + CellHeight * 2; y = y + CellHeight) // horizontal lines
			{
				var line = new Line();
				line.Stroke = Stroke;
				line.X1 = -OffsetX * 2;
				line.Y1 = y + OffsetY;
				line.X2 = ActualWidth + OffsetX * 2;
				line.Y2 = y + OffsetY;
				Children.Add(line);
				horizontalLines.Add(line);
			}
		}
	}
}
