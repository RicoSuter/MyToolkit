using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
	public sealed class ColorChooser : Control
	{
		public static Color[] DefaultColors { get { return new[] { Windows.UI.Colors.White, Windows.UI.Colors.Yellow, Windows.UI.Colors.Red, Windows.UI.Colors.Blue }; } }

		public ColorChooser()
		{
			DefaultStyleKey = typeof(ColorChooser);
		}

		public static readonly DependencyProperty SelectedColorProperty =
			DependencyProperty.Register("SelectedColor", typeof (Color), typeof (ColorChooser), new PropertyMetadata(default(Color)));

		public Color SelectedColor
		{
			get { return (Color) GetValue(SelectedColorProperty); }
			set { SetValue(SelectedColorProperty, value); }
		}

		public static readonly DependencyProperty ColorsProperty =
			DependencyProperty.Register("Colors", typeof(Color[]), typeof(ColorChooser), 
			new PropertyMetadata(DefaultColors));

		public Color[] Colors
		{
			get { return (Color[]) GetValue(ColorsProperty); }
			set { SetValue(ColorsProperty, value); }
		}
	}
}
