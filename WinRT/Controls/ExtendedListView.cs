using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace MyToolkit.Controls
{
	public class ExtendedListView : ListView
	{
		public ExtendedListView()
		{
			ItemContainerStyle = (Style)XamlReader.Load(
				@"<Style TargetType=""ListViewItem"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
					<Setter Property=""HorizontalContentAlignment"" Value=""Stretch""/>
				</Style>");
		}
	}
}