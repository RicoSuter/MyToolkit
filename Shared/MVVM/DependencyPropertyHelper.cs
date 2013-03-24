using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using MyToolkit.Utilities;

#if WINRT
using Windows.UI.Xaml;
#endif

namespace MyToolkit.MVVM
{
	public static class DependencyPropertyHelper
	{
		public static PropertyChangedCallback CallMethod<TView>(Action<TView> method) where TView : DependencyObject
		{
			return (obj, args) => method((TView)obj);
		}

		public static PropertyChangedCallback BindToViewModel<TViewModel>(Expression<Func<TViewModel, object>> property)
		{
			return BindToViewModel(ExpressionHelper.GetName(property));
		}

		public static PropertyChangedCallback BindToViewModel(string propertyName)
		{
			return (obj, args) =>
			{
				var vm = ((FrameworkElement)obj).Resources["viewModel"];
#if WINRT
				vm.GetType().GetTypeInfo().GetProperty(propertyName).SetValue(vm, args.NewValue, null);
#else
				vm.GetType().GetProperty(propertyName).SetValue(vm, args.NewValue, null);
#endif
			};
		}
	}
}