using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class ImageGenerator : SingleGenerator
	{
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlSettings settings)
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

				imgSource.ImageOpened += delegate { block.Update(settings.ActualWidth); };

				image.HorizontalAlignment = HorizontalAlignment.Left;
				image.Source = imgSource;
				image.Margin = new Thickness(0, settings.ParagraphMargin, 0, settings.ParagraphMargin); 

				if (width > 0)
					image.Width = width;
				if (height > 0)
					image.Height = height;

				settings.SizeChangedControls.Add(block);
				return image;
			}
			catch
			{
				return null;
			}
		}
	}
}