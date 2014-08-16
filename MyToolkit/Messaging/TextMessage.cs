//-----------------------------------------------------------------------
// <copyright file="TextMessage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Messaging
{
    public enum MessageButton
    {
        OK,
        OKCancel,
        YesNoCancel,
        YesNo
    }

    public enum MessageResult
    {
        OK,
        Cancel,
        Yes,
        No
    }

    public class TextMessage : CallbackMessage<MessageResult>
	{
		public TextMessage(string text)
			: this(text, "", MessageButton.OK) { }

		public TextMessage(string text, string title)
			: this(text, title, MessageButton.OK) { }

		public TextMessage(string text, string title, MessageButton button)
		{
			Text = text;
			Title = title;
			Button = button;
		}

		public string Title { get; set; }
		public string Text { get; set; }
		public MessageButton Button { get; set; }
	}
}
