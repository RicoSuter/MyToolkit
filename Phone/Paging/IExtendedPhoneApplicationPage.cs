using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyToolkit.Paging
{
	public interface IExtendedPhoneApplicationPage
	{
		event EventHandler<EventArgs> NavigatedTo;
		event EventHandler<EventArgs> NavigatedFrom;
	}
}
