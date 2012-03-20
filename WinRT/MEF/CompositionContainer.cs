using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace MyToolkit.MEF
{
	public class CompositionContainer
	{
		Dictionary<Type, object> objects = new Dictionary<Type, object>();

		public void ComposeParts(object obj)
		{
			ComposeParts(obj, obj.GetType());
		}

		private void ComposeParts(object obj, Type type)
		{
			var info = type.GetTypeInfo();
			foreach (var p in info.DeclaredProperties)
			{
				foreach (var a in p.GetCustomAttributes(typeof(ImportAttribute)))
				{
					var import = (ImportAttribute)a;
					p.SetValue(obj, objects[import.ContractType]);					
				}
			}

			if (info.BaseType != null)
				ComposeParts(obj, info.BaseType);
		}

		public void ComposeExportedValue<T>(T obj)
		{
			objects[typeof(T)] = obj; 
		}
	}
}
