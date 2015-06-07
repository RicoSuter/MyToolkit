//-----------------------------------------------------------------------
// <copyright file="TextMessage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Messaging
{
    /// <summary>Message to show a message box. </summary>
    public class TextMessage : CallbackMessage<MessageResult>
	{
        /// <summary>Initializes a new instance of the <see cref="TextMessage"/> class. </summary>
        /// <param name="text">The text. </param>
        public TextMessage(string text)
			: this(text, "", MessageButton.OK) { }

        /// <summary>Initializes a new instance of the <see cref="TextMessage"/> class. </summary>
        /// <param name="text">The text. </param>
        /// <param name="title">The title. </param>
		public TextMessage(string text, string title)
			: this(text, title, MessageButton.OK) { }

        /// <summary>Initializes a new instance of the <see cref="TextMessage"/> class. </summary>
        /// <param name="text">The text. </param>
        /// <param name="title">The title. </param>
        /// <param name="button">The shown buttons. </param>
        public TextMessage(string text, string title, MessageButton button)
		{
			Text = text;
			Title = title;
			Button = button;
		}

        /// <summary>Gets the message box title. </summary>
		public string Title { get; private set; }

        /// <summary>Gets the message box text. </summary>
        public string Text { get; private set; }
        
        /// <summary>Gets the message box buttons. </summary>
        public MessageButton Button { get; private set; }
	}

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
}
