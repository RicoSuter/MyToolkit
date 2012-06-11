using System.Collections.Generic;
using System.Windows;

namespace MyToolkit.Controls.HtmlTextBlockSource
{
	public interface IControlGenerator
	{
		DependencyObject[] Generate(HtmlNode node, IHtmlSettings settings);
	}
}