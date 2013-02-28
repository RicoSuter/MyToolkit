using System;

using MyToolkit.MVVM;

#if WINRT
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

#else
using System.Windows;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using MyToolkit.Paging;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class LinkGenerator : SingleControlGenerator
	{
#if WINRT
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlTextBlock textBlock)
		{
			try
			{
				var link = node.Attributes["href"];
				var label = node.Children[0].Value;

				var linkElement = new InlineUIContainer();
				var tb = new TextBlock();
				tb.Foreground = new SolidColorBrush(Colors.Blue);

				var underline = new Underline();
				underline.Inlines.Add(new Run { Text = label });
				tb.Inlines.Add(underline);

				var action = CreateLinkAction(tb, link, textBlock);
				tb.Tapped += delegate { action(); };

				linkElement.Child = tb;
				return linkElement;
			}
			catch
			{
				return null;
			}
		}

		protected virtual Action CreateLinkAction(TextBlock hyperlink, string link, IHtmlTextBlock textBlock)
		{
			return () => Launcher.LaunchUriAsync(new Uri(link)).AsTask().RunSynchronously();
		}
#else
		public override DependencyObject GenerateSingle(HtmlNode node, IHtmlTextBlock textBlock)
		{
			try
			{
				var link = node.Attributes["href"];
				var label = node.Children[0].Value;

				var hr = new Hyperlink();
				hr.MouseOverForeground = textBlock.Foreground; 
				hr.Foreground = textBlock.Foreground;
				hr.TextDecorations = TextDecorations.Underline;
				hr.Inlines.Add(label);

				var action = CreateLinkAction(hr, link, textBlock);
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

		protected virtual Action CreateLinkAction(Hyperlink hyperlink, string link, IHtmlTextBlock textBlock)
		{
			if (link.StartsWith("mailto:"))
				return () => new EmailComposeTask {To = link.Substring(7)}.Show();
			// 'tel:' removed because it needs CAP_PHONEDIALER capability! Use PhoneLinkGenerator from "- Other" directory. 

			try
			{
				var uri = link.StartsWith("http://") || link.StartsWith("https://") ?
					new Uri(link, UriKind.Absolute) : new Uri(textBlock.BaseUri, link);
				return () => new WebBrowserTask { Uri = uri }.Show();
			}
			catch (Exception)
			{

			}

			return () => { };
		}
#endif
	}
}