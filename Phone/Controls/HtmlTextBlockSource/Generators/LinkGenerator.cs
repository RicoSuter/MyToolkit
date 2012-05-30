using System;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using MyToolkit.Environment;
using MyToolkit.MVVM;
using MyToolkit.Paging;
using MyToolkit.Utilities;

namespace MyToolkit.Controls.HtmlTextBlockSource.Generators
{
	public class LinkGenerator : IControlGenerator
	{
		public DependencyObject Generate(HtmlNode node, IHtmlSettings settings)
		{
			try
			{
				var link = node.Attributes["href"];
				var label = node.Children[0].Value;

				var hr = new Hyperlink();
				hr.TextDecorations = TextDecorations.Underline;
				hr.Inlines.Add(label);

				var action = CreateLinkAction(hr, link, settings);
				var origAction = action;
				action = delegate
				{
					if (NavigationState.TryBeginNavigating())
						origAction();
				};

				hr.Command = new RelayCommand(action);	
				return hr;
			}
			catch
			{
				return null;
			}
		}

		protected virtual Action CreateLinkAction(Hyperlink hyperlink, string link, IHtmlSettings settings)
		{
			if (link.StartsWith("mailto:"))
				return () => new EmailComposeTask {To = link.Substring(7)}.Show();

			var uri = link.StartsWith("http://") || link.StartsWith("https://") ?
				new Uri(link, UriKind.Absolute) : new Uri(settings.BaseUri, link);
			return () => new WebBrowserTask { Uri = uri }.Show();
		}
	}
}