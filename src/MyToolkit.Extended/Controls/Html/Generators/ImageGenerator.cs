//-----------------------------------------------------------------------
// <copyright file="ImageGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WPF

using System;
using MyToolkit.Html;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#endif

namespace MyToolkit.Controls.Html.Generators
{
    /// <summary>Generates the UI element for an image (img) HTML tag.</summary>
    public class ImageGenerator : SingleControlGenerator
    {
        /// <summary>Creates a single UI element for the given HTML node and HTML view.</summary>
        /// <param name="node">The node.</param>
        /// <param name="htmlView">The text block.</param>
        /// <returns>The UI element.</returns>
        public override DependencyObject CreateControl(HtmlNode node, IHtmlView htmlView)
        {
            try
            {
                var imageUri = node.Attributes["src"];

                var height = 0;
                if (node.Attributes.ContainsKey("height"))
                    int.TryParse(node.Attributes["height"], out height);

                var width = 0;
                if (node.Attributes.ContainsKey("width"))
                    int.TryParse(node.Attributes["width"], out width);

                if (height == 1 && width == 1)
                    return null;

                var image = new Image();
                image.Width = 0;
                image.Height = 0;

                var bitmapImage = new BitmapImage(new Uri(imageUri));

                var imageBlock = new ImageBlock
                {
                    Image = image,
                    UserHeight = height,
                    UserWidth = width,
                    Source = bitmapImage
                };

                bitmapImage.ImageOpened += delegate { imageBlock.Update(htmlView.ActualWidth); };

                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.Source = bitmapImage;
                image.Margin = new Thickness(0, htmlView.ParagraphMargin, 0, htmlView.ParagraphMargin);

                if (width > 0)
                    image.Width = width;
                if (height > 0)
                    image.Height = height;

                htmlView.SizeDependentControls.Add(imageBlock);
                return new ContentPresenter { Content = image };
            }
            catch
            {
                return null;
            }
        }

        internal class ImageBlock : ISizeDependentControl
        {
            public Image Image { get; set; }

            public int UserWidth { get; set; }

            public int UserHeight { get; set; }

            public BitmapImage Source { get; set; }

            public void Update(double actualWidth)
            {
                if (Source.PixelWidth > 0 && actualWidth > 24)
                {
                    var width = actualWidth;

                    if (Source.PixelWidth < width)
                        width = Source.PixelWidth;

                    if (UserWidth < width && UserWidth != 0)
                        width = UserWidth;

                    Image.Width = width;
                    Image.Height = Source.PixelHeight * width / Source.PixelWidth;
                }
            }
        }
    }
}

#endif