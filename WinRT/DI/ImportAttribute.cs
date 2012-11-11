using System;

namespace MyToolkit.DI
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