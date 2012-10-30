using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation
{
	public class HtmlNode
	{
		public bool IsTag { get; private set; }
		public string Value { get; private set; }

		public Dictionary<string, string> Attributes { get; private set; }
		public List<HtmlNode> Children { get; private set; }

		public HtmlNode(string value, bool isTag, List<HtmlNode> children = null)
		{
			Children = children == null ? new List<HtmlNode>() : new List<HtmlNode>(children);
			Attributes = new Dictionary<string, string>();

			IsTag = isTag;
			SetValue(value);
		}

		private void SetValue(string value)
		{
			if (IsTag)
			{
				var index = value.IndexOf(' ');
				if (index > 0)
				{
					Value = value.Substring(0, index).ToLower();
					try
					{
						ParseAttributes(value.Substring(index));
					} catch { }
				}
				else
					Value = value.ToLower();
			}
			else
				Value = value;
		}

		private void ParseAttributes(string data)
		{
			var matches = Regex.Matches(data, "(.*?)='(.*?)'|(.*?)=\"(.*?)\"");
			foreach (Match m in matches)
			{
				var key = m.Groups[1].Value.Trim(' ', '\r', '\n');
				var val = m.Groups[2].Value.Trim(' ', '\r', '\n');
				if (!string.IsNullOrEmpty(key))
					Attributes.Add(key.ToLower(), val);
				else
				{
					key = m.Groups[3].Value.Trim(' ', '\r', '\n');
					val = m.Groups[4].Value.Trim(' ', '\r', '\n');
					if (!string.IsNullOrEmpty(key))
						Attributes.Add(key.ToLower(), val);
				}
			}
		}

		public DependencyObject[] GetLeaves(IHtmlTextBlock textBlock)
		{
			var list = new List<DependencyObject>();
			foreach (var c in Children)
			{
				var ctrl = c.GetControls(textBlock);
				if (ctrl != null)
					list.AddRange(ctrl);
				else
					list.AddRange(c.GetLeaves(textBlock));
			}
			return list.ToArray();
		}

		internal void AddChild(HtmlNode node)
		{
			Children.Add(node);
		}

		private bool loaded = false; 
		private DependencyObject[] controls;
		public DependencyObject[] GetControls(IHtmlTextBlock textBlock)
		{
			if (!loaded)
			{
				var value = IsTag ? Value : "text";
				var generator = textBlock.Generators.ContainsKey(value) ? textBlock.Generators[value] :
					(textBlock.Generators.ContainsKey("unknown") ? textBlock.Generators["unknown"] : null);
				if (generator != null)
					controls = generator.Generate(this, textBlock);
				loaded = true; 
			}
			return controls;
		}

		public void ToHtmlBlock()
		{
			var children = Children.ToList();
			Children.Clear();
			Children.Add(new HtmlNode("html", true, new List<HtmlNode> { new HtmlNode("p", true, children) }));
		}
	}
}