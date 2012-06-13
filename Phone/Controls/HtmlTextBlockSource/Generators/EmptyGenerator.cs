using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class EmptyGenerator : IControlGenerator
	{
		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			return new DependencyObject[] {};
		}
	}
}
