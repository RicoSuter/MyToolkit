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
			Tapped += delegate(object sender, TappedRoutedEventArgs args)
			{
				if (SuppressTappedEvents)
					args.Handled = true;
			};
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

		public static readonly DependencyProperty SuppressTappedEventsProperty =
			DependencyProperty.Register("SuppressTappedEvents", typeof (bool), typeof (ColorChooser), new PropertyMetadata(false));

		public bool SuppressTappedEvents
		{
			get { return (bool) GetValue(SuppressTappedEventsProperty); }
			set { SetValue(SuppressTappedEventsProperty, value); }
		}
	}
}
