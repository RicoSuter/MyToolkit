using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

namespace MyToolkit.Controls.HtmlTextBlockSource
{
	public class HtmlNode
	{
		public bool IsTag { get; private set; }
		public string Value { get; private set; }

		public Dictionary<string, string> Attributes { get { return attributes; } }
		public HtmlNode[] Children { get { return children.ToArray(); } }

		private readonly List<HtmlNode> children;
		private readonly Dictionary<string, string> attributes;  

		public HtmlNode(string value, bool isTag)
		{
			children = new List<HtmlNode>();
			attributes = new Dictionary<string, string>();

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
					attributes.Add(key.ToLower(), val);
				else
				{
					key = m.Groups[3].Value.Trim(' ', '\r', '\n');
					val = m.Groups[4].Value.Trim(' ', '\r', '\n');
					if (!string.IsNullOrEmpty(key))
						attributes.Add(key.ToLower(), val);
				}
			}
		}

		public DependencyObject[] GetLeaves(IHtmlSettings settings)
		{
			var list = new List<DependencyObject>();
			foreach (var c in children)
			{
				var ctrl = c.GetControl(settings);
				if (ctrl != null)
					list.Add(ctrl);
				else
					list.AddRange(c.GetLeaves(settings));
			}
			return list.ToArray();
		}

		internal void AddChild(HtmlNode node)
		{
			children.Add(node);
		}

		private bool loaded = false; 
		private DependencyObject control;
		public DependencyObject GetControl(IHtmlSettings settings)
		{
			if (!loaded)
			{
				var value = IsTag ? Value : "text";
				var generator = settings.Generators.ContainsKey(value) ? settings.Generators[value] :
					(settings.Generators.ContainsKey("unknown") ? settings.Generators["unknown"] : null);
				if (generator != null)
					control = generator.Generate(this, settings);
				loaded = true; 
			}
			return control;
		}
	}
}