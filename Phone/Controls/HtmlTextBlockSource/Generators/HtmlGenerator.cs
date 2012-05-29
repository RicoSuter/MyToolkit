using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class HtmlGenerator : IControlGenerator
	{
		public DependencyObject Generate(HtmlNode node, IHtmlSettings settings)
		{
			var panel = new StackPanel();
			foreach (var c in node.GetLeaves(settings))
				panel.Children.Add((UIElement)c);

			// correct first and last margin
			var elem = panel.Children.First() as FrameworkElement;
			if (elem != null)
			{
				elem = GetNonPanelElement(elem, true);
				if (elem != null)
					elem.Margin = new Thickness(elem.Margin.Left, 0, elem.Margin.Right, elem.Margin.Bottom);
			}
			
			elem = panel.Children.Last() as FrameworkElement;
			if (elem != null)
			{
				elem = GetNonPanelElement(elem, false);
				if (elem != null)
					elem.Margin = new Thickness(elem.Margin.Left, elem.Margin.Top, elem.Margin.Right, 0);
			}

			return panel;
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