using System.Windows.Controls;

namespace MyToolkit.UI
{
#if WP8
	public class PerformanceProgressBar : ProgressBar
	{

	}
#else
	public class PerformanceProgressBar : Microsoft.Phone.Controls.PerformanceProgressBar
	{

	}
#endif

	public class Panorama : Microsoft.Phone.Controls.Panorama
	{

	}
	public class PanoramaItem : Microsoft.Phone.Controls.PanoramaItem
	{

	}
}
