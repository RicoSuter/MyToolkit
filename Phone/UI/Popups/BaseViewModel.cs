using MyToolkit.Resources;

namespace MyToolkit.UI.Popups
{
	public class BaseViewModel
	{
		private readonly Strings strings = new Strings();
		public Strings Strings { get { return strings; } }
	}
}
