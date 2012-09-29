using System;
using MyToolkit.MVVM;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using MyToolkit.Paging;

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class PhoneLinkGenerator : LinkGenerator
	{
		protected override Action CreateLinkAction(Hyperlink hyperlink, string link, IHtmlTextBlock textBlock)
		{
			if (link.StartsWith("tel:"))
				return () => new PhoneCallTask { PhoneNumber = link.Substring(4) }.Show();
			return base.CreateLinkAction(hyperlink, link, textBlock);
		}
	}
}