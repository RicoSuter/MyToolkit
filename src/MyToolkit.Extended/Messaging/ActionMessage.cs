//-----------------------------------------------------------------------
// <copyright file="ActionMessage.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Messaging
{
	public class ActionMessage<T>
	{
		public Action<T> Action { get; set; }
	}
}
