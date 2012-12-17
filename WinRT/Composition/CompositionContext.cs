using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MyToolkit.Utilities;

namespace MyToolkit.Composition
{
	class PartDescription
	{
		public Type Key;
		public string Name;

		public Type Type;
		private object part;
		public object Part
		{
			set { part = value; }
			get
			{
				if (part == null)
					part = Activator.CreateInstance(Type);
				return part;
			}
		}
	}

	public class CompositionContext
	{
		private List<PartDescription> parts;

		public CompositionContext()
		{
			parts = new List<PartDescription>();
		}

		public void Clear()
		{
			parts.Clear();
		}

		public int AddPartsFromAssembly(Assembly assembly)
		{
			var count = 0; 
			foreach (var type in assembly.ExportedTypes)
			{
				var attr = type.GetTypeInfo().GetCustomAttribute<ExportAttribute>();
				if (attr != null)
				{
					AddPart(attr.Type, attr.Name, null, type);
					count++;
				}
			}
			return count; 
		}

		public bool AddPart<T>(T part)
		{
			return AddPart(typeof(T), null, part, part.GetType());
		}

		public bool AddPart<T>(string name, T part)
		{
			return AddPart(typeof(T), name, part, part.GetType());
		}
		
		public bool AddPart(Type importingType, object part)
		{
			return AddPart(importingType, null, part, part.GetType());
		}

		public bool AddPart(Type importingType, string name, object part)
		{
			return AddPart(importingType, name, part, part.GetType());
		}

		public bool AddPart<T>(Type instanceType)
		{
			return AddPart(typeof(T), null, null, instanceType);
		}

		public bool AddPart(Type importingType, Type instanceType)
		{
			return AddPart(importingType, null, null, instanceType);
		}

		public bool AddPart(Type importingType, string name, object part, Type instanceType)
		{
			RemovePart(instanceType, name);
			parts.Add(new PartDescription
			{
				Key = importingType,
				Type = instanceType, 
				Name = name, 
				Part = part
			});
			return true; 
		}

		public void RemovePart(Type instanceType, string name)
		{
			var oldPart = parts.SingleOrDefault(p => p.Type == instanceType && p.Name == name);
			if (oldPart != null)
				parts.Remove(oldPart);
		}

		public T GetPart<T>(string name = null)
		{
			return (T)GetPart(typeof (T), name);
		}

		public object GetPart(Type importingType, string name = null)
		{
			var part = parts.SingleOrDefault(p => p.Key == importingType && p.Name == name);
			if (part != null)
				return part.Part;
			return null; 
		}

		public IList GetParts(Type type)
		{
			var listType = typeof(List<>);
			listType = listType.MakeGenericType(type);

			var list = (IList)Activator.CreateInstance(listType);
			foreach (var part in parts.Where(p => p.Key == type).Select(p => p.Part))
				list.Add(part);
			return list; 
		}

		public IList<T> GetParts<T>()
		{
			return (IList<T>)GetParts(typeof(T));
		}

		public void SatisfyImports(object obj)
		{
			foreach (var property in obj.GetType().GetTypeInfo().GetAllProperties())
			{
				var attribute = property.GetCustomAttribute<ImportAttribute>();
				if (attribute != null)
				{
					var part = GetPart(attribute.Type, attribute.Name);
					if (part != null)
						property.SetValue(obj, part);
					else
						Debug.WriteLine("CompositionContext (Import): Part for property = " + 
							property.Name + " not found!");
				}

				var manyAttribute = property.GetCustomAttribute<ImportManyAttribute>();
				if (manyAttribute != null)
				{
					var parts = GetParts(manyAttribute.Type);
					if (parts != null)
						property.SetValue(obj, parts);
					else
						Debug.WriteLine("CompositionContext (ImportMany): Part for property = " +
							property.Name + " not found!");
				}
			}
		}
	}
}