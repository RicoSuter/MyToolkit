using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class ImageBlock : ISizeChangedControl
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