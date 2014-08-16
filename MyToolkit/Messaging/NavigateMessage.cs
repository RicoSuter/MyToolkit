//-----------------------------------------------------------------------
// <copyright file="NavigateMessage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Messaging
{
	public class NavigateMessage
	{
		public Type Page { get; private set; }

		public object Parameter { get; private set; }

		public NavigateMessage(Type page) : this(page, null) { } 

		public NavigateMessage(Type page, object parameter)
		{
			Page = page;
			Parameter = parameter; 
		}

		public NavigateMessage(Type page, params object[] parameters)
		{
			Page = page;
			Parameter = parameters;
		}
	}
}
