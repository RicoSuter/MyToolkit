using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MyToolkit.Controls.HtmlTextBlockSource
{
	public interface ISizeChangedControl
	{
		void Update(double actualWidth);
	}

	public interface IHtmlSettings
	{
		IDictionary<string, IControlGenerator> Generators { get; }
		List<ISizeChangedControl> SizeChangedControls { get; }

		Uri BaseUri { get; }
		int ParagraphMargin { get; }

		double FontSize { get; }
		FontFamily FontFamily { get; }

		double ActualWidth { get; }
	}
}
