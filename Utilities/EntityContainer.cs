using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyToolkit.Utilities
{
	public interface IEntity<U>
	{
		U ID { get; }
	}

	public class EntityContainer<T, U>
		where T : class, IEntity<U>
		where U : IEquatable<U>
	{
		private readonly ObservableCollection<T> collection = new ObservableCollection<T>();
		public ObservableCollection<T> Collection { get { return collection; } }
		
		public T Get(U id)
		{
			return collection.SingleOrDefault(n => n.ID.Equals(id));
		}

		public void Add(T item)
		{
			var c = Get(item.ID);
			if (c != null)
				collection.Remove(c);
			collection.Add(item);
		}

		public void Remove(T item)
		{
			var c = Get(item.ID);
			if (c != null)
				collection.Remove(c);
		}

		/// <summary>
		/// This works only if U == int
		/// </summary>
		/// <returns></returns>
		public int GenerateIdentity()
		{
			return IdentityGenerator.Generate(i => collection.Any(c => c.ID.Equals(i)));
		}
	}
}
