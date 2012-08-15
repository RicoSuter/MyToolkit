using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using MyToolkit.MVVM;

#if METRO
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
#else
using System.Windows;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using MyToolkit.Paging;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class UlGenerator : IControlGenerator
	{
		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			var list = new List<Grid>();
			foreach (var child in node.Children)
			{
				var grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

				var tb = new TextBlock();
				tb.Foreground = textBlock.Foreground;
				tb.FontSize = textBlock.FontSize;
				tb.FontFamily = textBlock.FontFamily;
				tb.Margin = new Thickness();
				tb.Text = "• ";

				grid.Children.Add(tb);
				Grid.SetColumn(tb, 0);

				var panel = new StackPanel();

				child.ToHtmlBlock();
				foreach (var c in child.GetLeaves(textBlock).OfType<UIElement>())
				{
					var frameworkElement = c as FrameworkElement;
					if (frameworkElement != null)
						frameworkElement.HorizontalAlignment = HorizontalAlignment.Stretch;
					panel.Children.Add(c);
				}

				grid.Children.Add(panel);
				Grid.SetColumn(panel, 1);

				list.Add(grid);
			}

			var first = list.FirstOrDefault();
			if (first != null)
				first.Margin = new Thickness(0, textBlock.ParagraphMargin, 0, 0);

			var last = list.LastOrDefault();
			if (last != null)
				last.Margin = new Thickness(0, 0, 0, textBlock.ParagraphMargin);

			return list.OfType<DependencyObject>().ToArray();
		}
	}
}