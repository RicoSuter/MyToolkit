using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	public class DataGridTemplatedCell : DataGridCell
	{
		public bool OnlyVisibleOnSelection { get; set; }

		public DataGridTemplatedCell(FrameworkElement control, bool onlyVisibleOnSelection)
			: base(control) 
		{
			OnlyVisibleOnSelection = onlyVisibleOnSelection; 
		}

		public override void OnSelectedChanged(bool isSelected)
		{
			if (OnlyVisibleOnSelection)
				Control.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed; 
		}
	}

	public class DataGridTemplatedColumn : DataGridColumn
	{
		private DataTemplate cellTemplate;
		public DataTemplate CellTemplate
		{
			get { return cellTemplate; }
			set { cellTemplate = value; }
		}

		private Binding order;
		public Binding Order
		{
			get { return order; }
			set { order = value; }
		}

		public override PropertyPath OrderPropertyPath
		{
			get { return order.Path; }
		}

		public bool OnlyVisibleOnSelection { get; set; }

		public override DataGridCell GenerateElement(object dataItem)
		{
			var control = new ContentControl();
			control.HorizontalContentAlignment = HorizontalAlignment.Stretch;
			control.VerticalContentAlignment = VerticalAlignment.Stretch;
			control.Content = dataItem;
			control.ContentTemplate = cellTemplate;

			if (OnlyVisibleOnSelection)
				control.Visibility = IsSelected ? Visibility.Visible : Visibility.Collapsed; 
			return new DataGridTemplatedCell(control, OnlyVisibleOnSelection);
		}
	}

	public class DataGridTextColumn : DataGridBoundColumn
	{
		private double? fontSize;
		[DefaultValue(double.NaN)]
		public double FontSize
		{
			get { return fontSize ?? Double.NaN; }
			set { fontSize = value; }
		}

		private FontStyle? fontStyle;
		public FontStyle FontStyle
		{
			get { return fontStyle ?? FontStyle.Normal; }
			set { fontStyle = value; }
		}

		private Brush foreground;
		public Brush Foreground
		{
			get { return foreground; }
			set { foreground = value; }
		}

		private Style style;
		public Style Style
		{
			get { return style; }
			set { style = value; }
		}

		public override DataGridCell GenerateElement(object dataItem)
		{
			var block = new TextBlock();
			block.VerticalAlignment = VerticalAlignment.Center;

			if (style != null)
				block.Style = style;

			if (fontSize.HasValue)
				block.FontSize = fontSize.Value;

			if (fontStyle.HasValue)
				block.FontStyle = fontStyle.Value;

			if (foreground != null)
				block.Foreground = foreground;

			if (Binding != null)
				block.SetBinding(TextBlock.TextProperty, Binding);

			return new DefaultDataGridCell(block);
		}
	}
}
