using System;

namespace MyToolkit.Composition
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ImportAttribute : Attribute
	{
		public Type Type;
		public string Name;

		public ImportAttribute(Type type)
		{
			Type = type; 
		}
	}
}