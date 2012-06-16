using System;
using System.Collections.Generic;
using System.Windows.Media;

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

		double FontSize { get; }
		FontFamily FontFamily { get; }

		double ActualWidth { get; }
	}
}
