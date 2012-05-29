using System;
using System.Windows;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using MyToolkit.MVVM;

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
				hr.Command = new RelayCommand(CreateLinkAction(hr, link, settings));	
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