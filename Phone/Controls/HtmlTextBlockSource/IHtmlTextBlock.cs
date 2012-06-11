using System.Windows;

namespace MyToolkit.Controls
{
	public interface IHtmlTextBlock
	{
		string Html { get; }
		UIElement HeaderItem { get; }
		void CallLoadedEvent();
	}
}