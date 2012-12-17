using System;

namespace MyToolkit.Composition
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ExportAttribute : Attribute
	{
		public Type Type;
		public string Name;

		public ExportAttribute(Type type)
		{
			Type = type; 
		}
	}
}