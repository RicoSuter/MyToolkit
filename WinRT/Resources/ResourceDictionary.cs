using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace MyToolkit.Resources
{
	public class ResourceDictionary : IReadOnlyDictionary<string, string>
	{
		private ResourceLoader resourceLoader;
		public ResourceDictionary()
		{
			resourceLoader = new ResourceLoader();
		}

		public ResourceDictionary(string name)
		{
			resourceLoader = new ResourceLoader(name);
		}

		public string GetString(string key)
		{
			return resourceLoader.GetString(key);
		}

		public string this[string key]
		{
			get { return resourceLoader.GetString(key); }
		}

		#region Unused

		public bool ContainsKey(string key)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerable<string> Keys
		{
			get { throw new NotImplementedException(); }
		}

		public bool TryGetValue(string key, out string value)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<string> Values
		{
			get { throw new NotImplementedException(); }
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
