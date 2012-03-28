namespace MyToolkit.Messaging
{
	public class TextMessage
	{
		public enum MessageButton
		{
			OK, 
			OKCancel
		}

		public enum MessageResult
		{
			OK,
			Cancel
		}

		public TextMessage(string text)
			: this(text, "", MessageButton.OK) { }

		public TextMessage(string text, string title)
			: this(text, title, MessageButton.OK) { }

		public TextMessage(string text, string title, MessageButton button)
		{
			Text = text;
			Title = title;
			Button = button;
			Result = MessageResult.OK;
		}

		public string Title { get; set; }
		public string Text { get; set; }
		public MessageButton Button { get; set; }

		public MessageResult Result { get; set; }
	}
}
