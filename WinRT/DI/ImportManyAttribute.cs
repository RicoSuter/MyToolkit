using System;

namespace MyToolkit.DI
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