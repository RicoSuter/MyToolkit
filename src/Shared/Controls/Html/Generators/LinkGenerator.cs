//-----------------------------------------------------------------------
// <copyright file="LinkGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using MyToolkit.Command;
using MyToolkit.Environment;
using MyToolkit.Html;
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

namespace MyToolkit.Controls.Html.Generators
{
    /// <summary>Generates the UI element for a link (a) HTML element.</summary>
    public class LinkGenerator : IControlGenerator
    {
        /// <summary>Gets or sets the text foreground brush (default: blue).</summary>
        public Brush Foreground { get; set; }

        /// <summary>Initializes a new instance of the <see cref="LinkGenerator"/> class.</summary>
        public LinkGenerator()
        {
            Foreground = new SolidColorBrush(Colors.Blue);
        }

        /// <summary>Creates the UI elements for the given HTML node and HTML view.</summary>
        /// <param name="node">The HTML node.</param>
        /// <param name="htmlView">The HTML view.</param>
        /// <returns>The UI elements.</returns>
        public DependencyObject[] CreateControls(HtmlNode node, IHtmlView htmlView)
        {
            try
            {
                var link = node.Attributes["href"];

                var hyperlink = new Hyperlink();
                hyperlink.Foreground = Foreground;

#if !WINRT
                hyperlink.MouseOverForeground = htmlView.Foreground;
                hyperlink.TextDecorations = TextDecorations.Underline;
#endif                
                foreach (var child in node.Children)
                {
                    var leaves = child.GetControls(htmlView).ToArray();
                    if (leaves.Length > 0)
                    {
                        foreach (var item in leaves)
                        {
                            if (item is Inline)
                                hyperlink.Inlines.Add((Inline)item);
                            else
                                hyperlink.Inlines.Add(new InlineUIContainer { Child = (UIElement) item });
                        }
                    }
                    else if (child is HtmlTextNode && !string.IsNullOrEmpty(((HtmlTextNode)child).Text))
                        hyperlink.Inlines.Add(new Run { Text = ((HtmlTextNode)child).Text });
                }

#if WINRT

                var action = CreateLinkAction(hyperlink, link, htmlView);
                hyperlink.Click += (sender, e) =>
                {
                    action();
                    ((Control)htmlView).Focus(FocusState.Programmatic);
                };

#else

                var action = CreateLinkAction(hyperlink, link, htmlView);
                hyperlink.Command = new RelayCommand(delegate
                {
                    if (!PhoneApplication.IsNavigating)
                        action();
                });

#endif
                
                return new DependencyObject[] { hyperlink };
            }
            catch
            {
                return node.GetChildControls(htmlView); // suppress link 
            }
        }

#if WINRT

        /// <summary>Creates the action to open the link after the user clicked on it.</summary>
        /// <param name="hyperlink">The hyperlink.</param>
        /// <param name="link">The link.</param>
        /// <param name="textBlock">The text block.</param>
        /// <returns>The action</returns>
        protected virtual Action CreateLinkAction(Hyperlink hyperlink, string link, IHtmlView textBlock)
        {
            return async () =>
            {
                try
                {
                    await Launcher.LaunchUriAsync(new Uri(link));
                }
                catch
                {
                    // Ignore malformed URIs. 
                }
            };
        }

#else // WP7SL/WP8SL
        
        /// <summary>Creates the action to open the link after the user clicked on it.</summary>
        /// <param name="hyperlink">The hyperlink.</param>
        /// <param name="link">The link.</param>
        /// <param name="textBlock">The text block.</param>
        /// <returns>The action</returns>
		protected virtual Action CreateLinkAction(Hyperlink hyperlink, string link, IHtmlView textBlock)
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