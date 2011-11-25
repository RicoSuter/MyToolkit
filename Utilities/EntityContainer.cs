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
	public interface IEntity
	{
		int ID { get; }
	}

	public class EntityContainer<T, U>
		where T : class, IEntity
		where U : IEquatable<U>
	{
		private readonly ObservableCollection<T> collection = new ObservableCollection<T>();
		public ObservableCollection<T> Collection { get { return collection; } }

		public T Get(string idAsString)
		{
			var id = int.Parse(idAsString);
			return collection.SingleOrDefault(n => n.ID == id);
		}

		public T Get(int id)
		{
			return collection.SingleOrDefault(n => n.ID == id);
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

		public int GenerateIdentity()
		{
			return IdentityGenerator.Generate(i => collection.Any(c => c.ID == i));
		}
	}
}
