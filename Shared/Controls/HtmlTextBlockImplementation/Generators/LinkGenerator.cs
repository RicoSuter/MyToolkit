using System;
using System.Linq;
using MyToolkit.Command;
using MyToolkit.Environment;
using MyToolkit.MVVM;

#if WINRT
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

using Windows.UI.Xaml.Input;
#else
using System.Windows;
using System.Windows.Documents;
using Microsoft.Phone.Tasks;
using MyToolkit.Paging;
using System.Windows.Media;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class LinkGenerator : IControlGenerator
	{
	    public LinkGenerator()
	    {
            Foreground = new SolidColorBrush(Colors.Blue);
	    }

        public Brush Foreground { get; set; }

#if WINRT
		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			try
			{
				var link = node.Attributes["href"];

				var block = new TextBlock();
                block.Foreground = Foreground;

				var element = new InlineUIContainer();
				element.Child = block;

				var hr = new Underline();
				foreach (var child in node.Children)
				{
					var leaves = child.GetLeaves(textBlock).ToArray();
					if (leaves.Length > 0)
					{
						foreach (var item in leaves.OfType<Inline>())
							hr.Inlines.Add(item);
					} else if (!string.IsNullOrEmpty(child.Value))
						hr.Inlines.Add(new Run { Text = child.Value });
				}
				block.Inlines.Add(hr);

				var action = CreateLinkAction(block, link, textBlock);
			    block.Tapped += (sender, e) =>
			    {
			        if (!e.Handled)
			        {
                        e.Handled = true;
                        action();
                    }
			    };
				return new DependencyObject[] { element };
			}
			catch
			{
				return node.GetLeaves(textBlock); // suppress link 
			}
		}

		protected virtual Action CreateLinkAction(TextBlock hyperlink, string link, IHtmlTextBlock textBlock)
		{
			return () => Launcher.LaunchUriAsync(new Uri(link));
		}
#else
		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			try
			{
				var link = node.Attributes["href"];

				var hr = new Hyperlink();
				hr.MouseOverForeground = textBlock.Foreground; 
				hr.Foreground = textBlock.Foreground;
				hr.TextDecorations = TextDecorations.Underline;

				foreach (var child in node.Children)
				{
					var leaves = child.GetLeaves(textBlock).ToArray();
					if (leaves.Length > 0)
					{
						foreach (var item in leaves.OfType<Inline>())
							hr.Inlines.Add(item);
					} else if (!string.IsNullOrEmpty(child.Value))
						hr.Inlines.Add(new Run { Text = child.Value });
				}

				var action = CreateLinkAction(hr, link, textBlock);
				var origAction = action;
				action = delegate
				{
					if (!PhoneApplication.IsNavigating)
						origAction();
				};

				hr.Command = new RelayCommand(action);
				return new DependencyObject[] { hr };
			}
			catch
			{
				return node.GetLeaves(textBlock); // suppress link 
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
					new Uri(link, UriKind.Absolute) : new Uri(textBlock.HtmlBaseUri, link);
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