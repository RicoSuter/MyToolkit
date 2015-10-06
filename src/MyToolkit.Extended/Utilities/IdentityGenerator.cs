//-----------------------------------------------------------------------
// <copyright file="IdentityGenerator.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Net;
using System.Windows;

namespace MyToolkit.Utilities
{
	public static class IdentityGenerator
	{
		public static int Generate(Predicate<int> isAlreadyInUsePredicate)
		{
			var rand = new Random((int)DateTime.Now.Ticks % int.MaxValue);
			while (true)
			{
				var result = rand.Next();
				if (!isAlreadyInUsePredicate(result))
					return result;
			}
		}
	}
}
