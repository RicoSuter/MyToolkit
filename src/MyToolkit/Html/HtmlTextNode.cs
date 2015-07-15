namespace MyToolkit.Html
{
    /// <summary>Represents an XML text node.</summary>
    public class HtmlTextNode : HtmlNode
    {
        /// <summary>Gets the text.</summary>
        public string Text { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="HtmlTextNode"/> class.</summary>
        /// <param name="text">The text.</param>
        public HtmlTextNode(string text)
        {
            Text = text; 
        }
    }
}