using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyToolkit.Utilities
{
	public static class DictionaryExtensions
	{
		public static Dictionary<string, object> DeepCopy(this Dictionary<string, object> dictionary) 
		{
			var output = new Dictionary<string, object>();
			foreach (var p in dictionary)
			{
				if (p.Value is Dictionary<string, object>)
					output[p.Key] = DeepCopy((Dictionary<string, object>)p.Value);
				else if (p.Value is List<Dictionary<string, object>>)
				{
					var list = (List<Dictionary<string, object>>)p.Value;
					output[p.Key] = list.Select(DeepCopy).ToList();
				}
				else
					output[p.Key] = p.Value;
			}
			return output;
		}
	}
}
