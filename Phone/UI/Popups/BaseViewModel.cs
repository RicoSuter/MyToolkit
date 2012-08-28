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
using MyToolkit.Resources;

namespace MyToolkit.UI.Popups.ViewModels
{
	public class BaseViewModel
	{
		private readonly Strings strings = new Strings();
		public Strings Strings { get { return strings; } }
	}
}
