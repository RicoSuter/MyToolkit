using System;
using System.Windows.Media;

namespace MyToolkit.UI.Popups
{
	public interface IPopupControl
	{
		event Action<object> Closed;
		void SetBackgroundColor(Color color);
		void GoBack();
	}
}