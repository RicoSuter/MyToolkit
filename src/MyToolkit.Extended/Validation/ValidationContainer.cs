#if WINRT

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Validation
{
	public class ValidationContainer : ContentPresenter
	{
		private readonly List<IValidationControl> _controls = new List<IValidationControl>();

		public static readonly DependencyProperty ExceptionsProperty =
			DependencyProperty.Register("Exceptions", typeof(IReadOnlyDictionary<IValidationControl, Exception>), typeof(ValidationContainer), 
			new PropertyMetadata(null));

		public IReadOnlyDictionary<IValidationControl, Exception> Exceptions
		{
			get { return (IReadOnlyDictionary<IValidationControl, Exception>)GetValue(ExceptionsProperty); }
			set { SetValue(ExceptionsProperty, value); }
		}

		public static readonly DependencyProperty IsValidProperty =
			DependencyProperty.Register("IsValid", typeof (bool), typeof (ValidationContainer), new PropertyMetadata(default(bool)));

		public bool IsValid
		{
			get { return (bool) GetValue(IsValidProperty); }
			set { SetValue(IsValidProperty, value); }
		}	

		public static void Register(IValidationControl ctrl)
		{
			var container = GetParent((DependencyObject)ctrl);
			if (container != null)
			{
				container.AddValidationControl(ctrl);
				container.Update();
				((FrameworkElement)ctrl).Loaded += delegate { container.AddValidationControl(ctrl); };
				((FrameworkElement)ctrl).Unloaded += delegate { container.RemoveValidationControl(ctrl); };
			}
		}

		private void AddValidationControl(IValidationControl ctrl)
		{
			if (!_controls.Contains(ctrl))
			{
				ctrl.ValidationChanged += OnValidationChanged;
				_controls.Add(ctrl);
				Update();
			}
		}

		private void RemoveValidationControl(IValidationControl ctrl)
		{
			if (_controls.Contains(ctrl))
			{
				ctrl.ValidationChanged -= OnValidationChanged;
				_controls.Remove(ctrl);
				Update();
			}
		}

		private void OnValidationChanged(object sender, Exception exception)
		{
			Update();
		}

		private void Update()
		{
			Exceptions = _controls
				.Where(c => c.ValidationException != null)
				.ToDictionary(c => c, c => c.ValidationException);
			IsValid = Exceptions.Count == 0; 
		}

		public static ValidationContainer GetParent(DependencyObject control)
		{
			var parent = VisualTreeHelper.GetParent(control);
			if (parent != null)
			{
				if (parent is ValidationContainer)
					return (ValidationContainer) parent;
				return GetParent(parent);
			}
			return null; 
		}
	}
}

#endif