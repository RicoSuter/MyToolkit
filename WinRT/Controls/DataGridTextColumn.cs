using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
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
