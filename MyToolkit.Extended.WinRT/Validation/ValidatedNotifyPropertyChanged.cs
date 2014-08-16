using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using MyToolkit.Data;
using MyToolkit.Model;
using MyToolkit.MVVM;
using MyToolkit.Utilities;

namespace MyToolkit.Validation
{
	public class ValidatedNotifyPropertyChanged<TClass> : ObservableObject, INotifyDataErrorInfo 
		where TClass : ValidatedNotifyPropertyChanged<TClass>
	{
		private readonly List<ValidationResult> _errors = new List<ValidationResult>();
		
		public void ClearError<T>(Expression<Func<TClass, T>> property)
		{
			ClearError(ExpressionHelper.GetName(property));
		}

		public void ClearError(string propertyName)
		{
			SetError(propertyName, null);
		}

		public void SetError<T>(Expression<Func<TClass, T>> property, object error)
		{
			SetError(ExpressionHelper.GetName(property), error);
		}

		public void SetError(string propertyName, object error)
		{
			var result = _errors.SingleOrDefault(e => e.Property == propertyName);
			if (result == null)
			{
				if (error == null)
					return;

				result = new ValidationResult {Property = propertyName, Error = error};
				_errors.Add(result);
			}
			else
			{
				if (error == null)
					_errors.Remove(result);
				else
					result.Error = error;
			}

			var copy = ErrorsChanged;
			if (copy != null)
				copy(this, new DataErrorsChangedEventArgs(propertyName));
		}

		public IEnumerable<ValidationResult> GetErrors<T>(Expression<Func<TClass, T>> property)
		{
			return (IEnumerable<ValidationResult>) GetErrors(ExpressionHelper.GetName(property));
		}

		public IEnumerable GetErrors(string propertyName)
		{
			return _errors.Where(e => e.Error != null).ToList(); 
		}

		public bool HasErrors { get; private set; }
		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
	}

	public class ValidationResult
	{
		public string Property { get; set; }
		public object Error { get; set; }
	}
}
