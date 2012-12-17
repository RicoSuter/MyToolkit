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