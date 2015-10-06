//-----------------------------------------------------------------------
// <copyright file="ColorChooser.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
    /// <summary>Provides a simple color chooser. </summary>
    public sealed class ColorChooser : Control
    {
        public static Color[] DefaultColors
        {
            get
            {
                return new[]
		        {
		            Windows.UI.Colors.White, 
                    Windows.UI.Colors.Yellow, 
                    Windows.UI.Colors.Red, 
                    Windows.UI.Colors.Blue
		        };
            }
        }

        public ColorChooser()
        {
            DefaultStyleKey = typeof(ColorChooser);
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorChooser), new PropertyMetadata(default(Color)));

        /// <summary>Gets or sets the selected color. </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register("Colors", typeof(Color[]), typeof(ColorChooser), new PropertyMetadata(DefaultColors));

        /// <summary>Gets or sets the available colors in the chooser. </summary>
        public Color[] Colors
        {
            get { return (Color[])GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }
    }
}

#endif