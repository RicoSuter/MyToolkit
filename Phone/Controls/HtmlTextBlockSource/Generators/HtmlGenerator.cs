using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class HtmlGenerator : IControlGenerator
	{
		public DependencyObject[] Generate(HtmlNode node, IHtmlSettings settings)
		{
			var items = node.GetLeaves(settings).ToList();

			if (items.Count == 1 && items[0] is Panel)
				items = new List<DependencyObject> { items[0] };

			if (items.Count > 0)
			{
				var elem = items.First() as FrameworkElement;
				if (elem != null)
				{
					elem = GetNonPanelElement(elem, true);
					if (elem != null)
						elem.Margin = new Thickness(elem.Margin.Left, 0, elem.Margin.Right, elem.Margin.Bottom);
				}

				elem = items.Last() as FrameworkElement;
				if (elem != null)
				{
					elem = GetNonPanelElement(elem, false);
					if (elem != null)
						elem.Margin = new Thickness(elem.Margin.Left, elem.Margin.Top, elem.Margin.Right, 0);
				}
			}

			return items.ToArray();
		}

		private FrameworkElement GetNonPanelElement(FrameworkElement element, bool first)
		{
			var panel = element as Panel;
			if (panel != null)
			{
				if (panel.Children.Count > 0)
				{
					var next = (first ? panel.Children.First() : panel.Children.Last()) as FrameworkElement;
					return next != null ? GetNonPanelElement(next, first) : null;
				}
				return null; 
			}
			return element; 
		}
	}
}