using System;
using System.Collections.Generic;

#if WINRT
using Windows.UI.Xaml.Media;
#else
using System.Windows.Media;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation
{
	public interface IHtmlTextBlock
	{
		string Html { get; }
		void CallLoadedEvent();

		IDictionary<string, IControlGenerator> Generators { get; }
		List<ISizeDependentControl> SizeDependentControls { get; }

		Uri BaseUri { get; }
		int ParagraphMargin { get; }

		Brush Foreground { get; }
		Brush Background { get; }
		double FontSize { get; }
		FontFamily FontFamily { get; }

		double ActualWidth { get; }
	}
}
