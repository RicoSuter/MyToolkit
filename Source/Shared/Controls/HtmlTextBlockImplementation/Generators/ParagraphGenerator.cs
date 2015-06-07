using System.Collections.Generic;
using System.Linq;
#if WINRT
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
#endif

namespace MyToolkit.Controls.HtmlTextBlockImplementation.Generators
{
	public class ParagraphGenerator : IControlGenerator
	{
		public double FontSize { get; set; }
		public FontFamily FontFamily { get; set; }
		public Brush Foreground { get; set; }
		public FontStyle FontStyle { get; set; }
		public FontWeight FontWeight { get; set; }

		public ParagraphGenerator()
		{
#if WINRT
			FontStyle = FontStyle.Normal;
			FontWeight = FontWeights.Normal; 
#else
			FontStyle = FontStyles.Normal;
			FontWeight = FontWeights.Normal;
#endif

			UseTextSplitting = true; 
		}

		/// <summary>
		/// Gets or sets a flag whether text should be split up in multiple RichTextBlocks to avoid the 2048px element size limit (default true).
		/// </summary>
		public bool UseTextSplitting { get; set; } 

		public DependencyObject[] Generate(HtmlNode node, IHtmlTextBlock textBlock)
		{
			var list = new List<DependencyObject>();

			var addTopMargin = true; 
			var current = new List<Inline>();
			foreach (var c in node.GetLeaves(textBlock))
			{
				if (c is Run && UseTextSplitting && ((Run)c).Text.Contains("\n")) // used to avoid 2048px max control size
				{
					// split text
					var run = (Run) c;
					var splits = run.Text.Split('\n');

					// join some splits to avoid small junks 
					var currentSplit = "";
					var newSplits = new List<string>();
					for (var i = 0; i < splits.Length; i++)
					{
						var split = splits[i];
						if (i != 0 && currentSplit.Length + split.Length > 16)
						{
							newSplits.Add(currentSplit);
							currentSplit = split;
						}
						else
							currentSplit += (i != 0 ? "\n" : "") + split;
					}
					newSplits.Add(currentSplit);

					// create multiple text blocks
					splits = newSplits.ToArray();
					for (var i = 0; i < splits.Length; i++)
					{
						var split = splits[i];
						current.Add(new Run { Text = split });
						if (i < splits.Length - 1) // dont create for last
							CreateTextBox(list, current, textBlock, i == 0 && addTopMargin, false);
					}
                    addTopMargin = list.Count == 0; 
				} else if (c is Inline)
					current.Add((Inline)c);
				else
				{
					CreateTextBox(list, current, textBlock, addTopMargin, true);
					list.Add(c);
					addTopMargin = true; 
				}
			}
			CreateTextBox(list, current, textBlock, addTopMargin, true);

			if (list.Count == 0)
				return null;
			return list.ToArray();
		}

		private void CreateTextBox(List<DependencyObject> list, List<Inline> current, IHtmlTextBlock textBlock, 
			bool addTopMargin, bool addBottomMargin)
		{
			if (current.Count > 0)
			{
				var p = new Paragraph();
				foreach (var r in current)
					p.Inlines.Add(r);

#if !WINRT
				var tb = new RichTextBox();
				tb.Background = textBlock.Background;
				tb.Margin = new Thickness(-12, addTopMargin ? textBlock.ParagraphMargin : 0, -12, addBottomMargin ? textBlock.ParagraphMargin : 0);
#else
				var tb = new RichTextBlock();
				tb.IsTextSelectionEnabled = false; // TODO: when to add topmargin
				tb.Margin = new Thickness(0, /*addTopMargin ? textBlock.ParagraphMargin :*/ 0, 0, addBottomMargin ? textBlock.ParagraphMargin : 0);
#endif
				tb.Blocks.Add(p);
				tb.Foreground = Foreground ?? textBlock.Foreground;
				tb.FontSize = FontSize == 0 ? textBlock.FontSize : FontSize;
				tb.FontFamily = FontFamily ?? textBlock.FontFamily;
				tb.FontStyle = FontStyle;
				tb.FontWeight = FontWeight;
				
				list.Add(tb);
				current.Clear();
			}
		}
	}
}