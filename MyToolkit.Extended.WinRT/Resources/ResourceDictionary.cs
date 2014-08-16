using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Foundation.Metadata;

namespace MyToolkit.Resources
{
	public class ResourceDictionary : IReadOnlyDictionary<string, string>
	{
		private readonly ResourceLoader _resourceLoader;

		public ResourceDictionary()
		{
			_resourceLoader = ResourceLoader.GetForCurrentView();
		}

		public ResourceDictionary(string name)
		{
            _resourceLoader = ResourceLoader.GetForCurrentView(name);
		}

		public string GetString(string key)
		{
			return _resourceLoader.GetString(key);
		}

		public string this[string key]
		{
			get { return _resourceLoader.GetString(key); }
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
