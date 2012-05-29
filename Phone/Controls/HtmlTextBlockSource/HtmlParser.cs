using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MyToolkit.Controls.HtmlTextBlockSource.Generators;

namespace MyToolkit.Controls.HtmlTextBlockSource
{
	public class HtmlParser
	{
		public static Dictionary<string, IControlGenerator> GetDefaultGenerators()
		{
			var list = new Dictionary<string, IControlGenerator>();
			list.Add("p", new ParagraphGenerator());
			list.Add("html", new HtmlGenerator());
			list.Add("strong", new StrongGenerator());
			list.Add("b", list["strong"]);
			list.Add("text", new TextGenerator());
			list.Add("em", new EmGenerator());
			list.Add("i", list["em"]);
			list.Add("a", new LinkGenerator());
			list.Add("img", new ImageGenerator());
			return list; 
		}

		public HtmlNode Parse(string html)
		{
			html = html.Replace("\r", "").Replace("\n", "").Replace("\t", "");
			html = html.Replace("<br>", "\n").Replace("<br />", "\n").Replace("<br/>", "\n");
			html = html.Replace("<BR>", "\n").Replace("<BR />", "\n").Replace("<BR/>", "\n");
			html = "<p>" + html + "</p>";

			var matches = Regex.Matches(html, "<(.*?)>|</(.*?)>|<(.*?)/>|([^<>]*)");
			var tokens = matches.OfType<Group>().Select(m => m.Value).ToArray();

			var root = new HtmlNode("html", true);
			var stack = new Stack<HtmlNode>();
			stack.Push(root);

			foreach (var token in tokens)
			{
				if (token.StartsWith("</") && token.EndsWith(">")) // end tag
				{
					var value = token.Substring(2, token.Length - 3);
					var node = stack.Peek();
					if (node.Value == value)
						stack.Pop();
	//				else
//						throw new Exception("malformed html");
				}
				else if (token.StartsWith("<") && token.EndsWith("/>")) // full tag
				{
					var value = token.Substring(1, token.Length - 3);
					stack.Peek().AddChild(new HtmlNode(value, true));
				}
				else if (token.StartsWith("<") && token.EndsWith(">")) // start tag
				{
					var value = token.Substring(1, token.Length - 2);
					var node = new HtmlNode(value, true);
					stack.Peek().AddChild(node);
					stack.Push(node);
				}
				else if (!string.IsNullOrEmpty(token)) // text
					stack.Peek().AddChild(new HtmlNode(token, false));
			}

			return root;
		}
	}
}
