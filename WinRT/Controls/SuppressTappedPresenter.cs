using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
	public class SuppressTappedPresenter : ContentPresenter
	{
		public SuppressTappedPresenter()
		{
			Tapped += delegate(object sender, TappedRoutedEventArgs args) { args.Handled = true; };
		}
	}
}
