#if METRO
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class ImageBlock : ISizeDependentControl
	{
		public Image Image { get; set; }
		public int UserWidth { get; set; }
		public int UserHeight { get; set; }
		public BitmapImage Source { get; set; }

		public void Update(double actualWidth)
		{
			if (Source.PixelWidth > 0 && actualWidth > 24)
			{
				var width = (int)actualWidth;
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