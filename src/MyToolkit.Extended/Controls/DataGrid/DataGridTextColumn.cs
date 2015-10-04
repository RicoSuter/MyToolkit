//-----------------------------------------------------------------------
// <copyright file="DataGridTextColumn.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.ComponentModel;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
    public class DataGridTextColumn : DataGridBoundColumn
    {
        private double? _fontSize;
        private FontStyle? _fontStyle;
        private Brush _foreground;
        private Style _style;

        /// <summary>Gets or sets the font size. </summary>
        [DefaultValue(double.NaN)]
        public double FontSize
        {
            get { return _fontSize ?? Double.NaN; }
            set { _fontSize = value; }
        }

        /// <summary>Gets or sets the font style. </summary>
        public FontStyle FontStyle
        {
            get { return _fontStyle ?? FontStyle.Normal; }
            set { _fontStyle = value; }
        }

        /// <summary>Gets or sets the foreground. </summary>
        public Brush Foreground
        {
            get { return _foreground; }
            set { _foreground = value; }
        }

        /// <summary>Gets or sets the style. </summary>
        public Style Style
        {
            get { return _style; }
            set { _style = value; }
        }

        /// <summary>Generates the cell for the given item. </summary>
        /// <param name="dataItem">The item to generate the cell for. </param>
        /// <returns>The <see cref="DataGridCellBase"/>. </returns>
        public override DataGridCellBase CreateCell(object dataItem)
        {
            var block = new TextBlock();
            block.VerticalAlignment = VerticalAlignment.Center;

            if (_style != null)
                block.Style = _style;

            if (_fontSize.HasValue)
                block.FontSize = _fontSize.Value;

            if (_fontStyle.HasValue)
                block.FontStyle = _fontStyle.Value;

            if (_foreground != null)
                block.Foreground = _foreground;

            if (Binding != null)
                block.SetBinding(TextBlock.TextProperty, Binding);

            return new DefaultDataGridCell(block);
        }
    }
}

#endif