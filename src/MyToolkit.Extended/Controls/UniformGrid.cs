//-----------------------------------------------------------------------
// <copyright file="UniformGrid.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
#if SL4 || SL5 || WP7 || WP8
using System.Windows;
using System.Windows.Controls;
#elif WPF
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif 

namespace MyToolkit.Controls
{
	/// <summary>Panel which arranges children in a grid with all equal cell sizes. </summary>
	public class UniformGrid : Panel
	{
		private int ComputedRows { get; set; }
		private int ComputedColumns { get; set; }

		public static readonly DependencyProperty FirstColumnProperty = DependencyProperty.Register("FirstColumn", typeof(int), typeof(UniformGrid), 
			new PropertyMetadata(0, OnPositiveIntegerChanged));

		public int FirstColumn
		{
			get { return (int)GetValue(FirstColumnProperty); }
			set { SetValue(FirstColumnProperty, value); }
		}

		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register("Columns", typeof(int), typeof(UniformGrid),
			new PropertyMetadata(0, OnPositiveIntegerChanged));

		public int Columns
		{
			get { return (int)GetValue(ColumnsProperty); }
			set { SetValue(ColumnsProperty, value); }
		}

		private static void OnPositiveIntegerChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			if (!(e.NewValue is int) || (int)e.NewValue < 0)
				o.SetValue(e.Property, e.OldValue);
		}

		public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(int), typeof(UniformGrid), 
			new PropertyMetadata(0, OnPositiveIntegerChanged));

		public int Rows
		{
			get { return (int)GetValue(RowsProperty); }
			set { SetValue(RowsProperty, value); }
		}

		protected override Size MeasureOverride(Size constraint)
		{
			UpdateComputedValues();

			var childConstraint = new Size(constraint.Width / ComputedColumns, constraint.Height / ComputedRows);
			var maxChildDesiredWidth = 0.0;
			var maxChildDesiredHeight = 0.0;

			for (int i = 0, count = Children.Count; i < count; ++i)
			{
				var child = Children[i];
				child.Measure(childConstraint);
				
				var childDesiredSize = child.DesiredSize;
				if (maxChildDesiredWidth < childDesiredSize.Width)
					maxChildDesiredWidth = childDesiredSize.Width;

				if (maxChildDesiredHeight < childDesiredSize.Height)
					maxChildDesiredHeight = childDesiredSize.Height;
			}
			return new Size((maxChildDesiredWidth * ComputedColumns), (maxChildDesiredHeight * ComputedRows));
		}

		protected override Size ArrangeOverride(Size arrangeSize)
		{
			var childBounds = new Rect(0, 0, arrangeSize.Width / ComputedColumns, arrangeSize.Height / ComputedRows);
			var xStep = childBounds.Width;
			var xBound = arrangeSize.Width - 1.0;
			childBounds.X += childBounds.Width * FirstColumn;

			foreach (var child in Children.OfType<UIElement>())
			{
				child.Arrange(childBounds);
				if (child.Visibility != Visibility.Collapsed)
				{
					childBounds.X += xStep;
					if (childBounds.X >= xBound)
					{
						childBounds.Y += childBounds.Height;
						childBounds.X = 0;
					}
				}
			}

			return arrangeSize;
		}

		private void UpdateComputedValues()
		{
			ComputedColumns = Columns;
			ComputedRows = Rows;

			if (FirstColumn >= ComputedColumns)
				FirstColumn = 0;

			if ((ComputedRows == 0) || (ComputedColumns == 0))
			{
				var nonCollapsedCount = 0;
				for (int i = 0, count = Children.Count; i < count; ++i)
				{
					var child = Children[i];
					if (child.Visibility != Visibility.Collapsed)
						nonCollapsedCount++;
				}

				if (nonCollapsedCount == 0)
					nonCollapsedCount = 1;

				if (ComputedRows == 0)
				{
					if (ComputedColumns > 0)
						ComputedRows = (nonCollapsedCount + FirstColumn + (ComputedColumns - 1)) / ComputedColumns;
					else
					{
						ComputedRows = (int)Math.Sqrt(nonCollapsedCount);
						if ((ComputedRows * ComputedRows) < nonCollapsedCount)
							ComputedRows++;

						ComputedColumns = ComputedRows;
					}
				}
				else if (ComputedColumns == 0)
					ComputedColumns = (nonCollapsedCount + (ComputedRows - 1)) / ComputedRows;
			}
		}
	}
}