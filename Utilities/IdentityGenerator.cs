using System;
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
	public static class IdentityGenerator
	{
		public static int Generate(Predicate<int> isAlreadyInUsePredicate)
		{
			var rand = new Random((int)DateTime.Now.Ticks % int.MaxValue);
			while (true)
			{
				var result = rand.Next();
				if (!isAlreadyInUsePredicate(result))
					return result;
			}
		}
	}
}
