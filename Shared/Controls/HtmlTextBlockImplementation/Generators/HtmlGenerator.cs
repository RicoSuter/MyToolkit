using System.Collections.Generic;
using System.Linq;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class HtmlGenerator : IControlGenerator
	{
		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			var items = node.GetLeaves(textBlock).ToList();

			if (items.Count == 1 && items[0] is Panel)
				items = new List<DependencyObject> { items[0] };

			if (items.Count > 0)
			{
				var elem = items.First() as FrameworkElement;
				if (elem != null)
					elem.Margin = new Thickness(elem.Margin.Left, 0, elem.Margin.Right, elem.Margin.Bottom);

				elem = items.Last() as FrameworkElement;
				if (elem != null)
					elem.Margin = new Thickness(elem.Margin.Left, elem.Margin.Top, elem.Margin.Right, 0);
			}

			return items.ToArray();
		}
	}
}