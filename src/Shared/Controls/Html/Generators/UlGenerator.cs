//-----------------------------------------------------------------------
// <copyright file="UlGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Collections.Generic;
using MyToolkit.Html;

#if WINRT
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;
#endif

namespace MyToolkit.Controls.Html.Generators
{
    /// <summary>Generator for the UL HTML element (unordered list).</summary>
    public class UlGenerator : IControlGenerator
	{
        /// <summary>Initializes a new instance of the <see cref="UlGenerator"/> class.</summary>
        public UlGenerator()
	    {
            BulletSymbol = "•";
	    }

        /// <summary>Gets or sets the bullet symbol for a list element.</summary>
        public string BulletSymbol { get; set; }

        /// <summary>Creates the UI elements for the given HTML node and HTML view.</summary>
        /// <param name="node">The HTML node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI elements.</returns>
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView htmlView)
		{
			var controls = new List<Grid>();
			foreach (var child in node.Children.OfType<HtmlTagNode>().Where(c => c.Name == "li"))
			{
				var grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(20)});
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

				var textBlock = CreateBulletSymbol(htmlView);
                			    grid.Children.Add(textBlock);
				Grid.SetColumn(textBlock, 0);

				var panel = new StackPanel();

				child.WrapWithHtmlTag();
				foreach (var c in child.GetChildControls(htmlView).OfType<UIElement>())
				{
					var frameworkElement = c as FrameworkElement;
					if (frameworkElement != null)
						frameworkElement.HorizontalAlignment = HorizontalAlignment.Stretch;

					panel.Children.Add(c);
				}

				grid.Children.Add(panel);
				Grid.SetColumn(panel, 1);

				controls.Add(grid);
			}

			AdjustMargins(htmlView, controls);
            return controls.OfType<DependencyObject>().ToArray();
		}

        private TextBlock CreateBulletSymbol(IHtmlView htmlView)
        {
            var textBlock = new TextBlock();
            textBlock.Foreground = htmlView.Foreground;
            textBlock.FontSize = htmlView.FontSize;
            textBlock.FontFamily = htmlView.FontFamily;
            textBlock.Margin = new Thickness();
            textBlock.Text = BulletSymbol;
            return textBlock;
        }

        private void AdjustMargins(IHtmlView htmlView, List<Grid> controls)
        {
            var firstControl = controls.FirstOrDefault();
            if (firstControl != null)
                firstControl.Margin = new Thickness(0, htmlView.ParagraphMargin, 0, 0);

            var lastControl = controls.LastOrDefault();
            if (lastControl != null)
                lastControl.Margin = new Thickness(0, 0, 0, htmlView.ParagraphMargin);
        }
	}
}