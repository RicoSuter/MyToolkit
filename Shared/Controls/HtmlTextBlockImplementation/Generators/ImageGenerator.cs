using System;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class ImageGenerator : SingleControlGenerator
	{
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlTextBlock textBlock)
		{
			try
			{
				var uri = node.Attributes["src"];

				var height = 0;
				if (node.Attributes.ContainsKey("height"))
					int.TryParse(node.Attributes["height"], out height);

				var width = 0;
				if (node.Attributes.ContainsKey("width"))
					int.TryParse(node.Attributes["width"], out width);

				if (height == 1 && width == 1)
					return null; 

				var image = new Image();
				var imgSource = new BitmapImage(new Uri(uri));
				var block = new ImageBlock
				            	{
				            		Image = image, 
				            		UserHeight = height, 
				            		UserWidth = width, 
				            		Source = imgSource
				            	};

				imgSource.ImageOpened += delegate { block.Update(textBlock.ActualWidth); };

				image.HorizontalAlignment = HorizontalAlignment.Left;
				image.Source = imgSource;
				image.Margin = new Thickness(0, textBlock.ParagraphMargin, 0, textBlock.ParagraphMargin); 

				if (width > 0)
					image.Width = width;
				if (height > 0)
					image.Height = height;

				textBlock.SizeDependentControls.Add(block);
				return image;
			}
			catch
			{
				return null;
			}
		}
	}
}