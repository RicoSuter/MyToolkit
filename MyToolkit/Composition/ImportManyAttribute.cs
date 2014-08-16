//-----------------------------------------------------------------------
// <copyright file="ImportManyAttribute.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Composition
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ImportManyAttribute : Attribute
	{
		public Type Type;
		public ImportManyAttribute(Type type)
		{
			Type = type; 
		}
	}
}