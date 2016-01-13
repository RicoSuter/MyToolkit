//-----------------------------------------------------------------------
// <copyright file="DataGridTextColumn.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.ComponentModel;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
    public class DataGridTextColumn : DataGridBoundColumn
    {
        public static readonly DependencyProperty StyleProperty = DependencyProperty.Register(
            "Style", typeof (Style), typeof (DataGridTextColumn), new PropertyMetadata(default(Style)));

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(DataGridTextColumn), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
            "FontStyle", typeof(FontStyle), typeof(DataGridTextColumn), new PropertyMetadata(default(FontStyle)));

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(Brush), typeof(DataGridTextColumn), new PropertyMetadata(default(Brush)));

        /// <summary>Gets or sets the style.</summary>
        public Style Style
        {
            get { return (Style) GetValue(StyleProperty); }
            set { SetValue(StyleProperty, value); }
        }

        /// <summary>Gets or sets the size of the font.</summary>
        public double FontSize
        {
            get { return (double) GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>Gets or sets the font style. </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle) GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>Gets or sets the foreground brush.</summary>
        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>Generates the cell for the given item. </summary>
        /// <param name="dataGrid"></param>
        /// <param name="dataItem">The item to generate the cell for. </param>
        /// <returns>The <see cref="DataGridCellBase"/>. </returns>
        public override DataGridCellBase CreateCell(DataGrid dataGrid, object dataItem)
        {
            var block = new TextBlock();
            block.VerticalAlignment = VerticalAlignment.Center;

            if (FontSize <= 0)
                FontSize = dataGrid.FontSize;

            CreateBinding(StyleProperty, "Style", block, TextBlock.StyleProperty);
            CreateBinding(FontStyleProperty, "FontStyle", block, TextBlock.FontStyleProperty);
            CreateBinding(FontSizeProperty, "FontSize", block, TextBlock.FontSizeProperty);
            CreateBinding(ForegroundProperty, "Foreground", block, TextBlock.ForegroundProperty);

            if (Binding != null)
                block.SetBinding(TextBlock.TextProperty, Binding);

            return new DefaultDataGridCell(block);
        }
    }
}

#endif