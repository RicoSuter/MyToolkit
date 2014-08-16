//-----------------------------------------------------------------------
// <copyright file="LoadingMessage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Messaging
{
	public class LoadingMessage
	{
		public LoadingMessage(bool isLoading)
		{
			IsLoading = isLoading; 
		}

		public bool IsLoading { get; set; }
	}
}
