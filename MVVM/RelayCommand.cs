using System;
using System.Diagnostics;

#if METRO
using Windows.UI.Xaml.Input;
#else
using System.Windows.Input;
#endif

namespace MyToolkit.MVVM
{
	public class RelayCommand : NotifyPropertyChanged, ICommand // NotifyPropertyChanged used allow bindings to CanExecute property
	{
		private readonly Action execute;
		private readonly Func<bool> canExecute;

		public RelayCommand(Action execute)
			: this(execute, null) { }

		public RelayCommand(Action execute, Func<bool> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			this.execute = execute;
			this.canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return canExecute == null ? true : canExecute();
		}

		public void Execute(object parameter)
		{
			execute();
		}

		public void RaiseCanExecuteChanged()
		{
			RaisePropertyChanged("CanExecute");
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, new EventArgs());
		}

#if METRO
		public event Windows.UI.Xaml.EventHandler CanExecuteChanged;
#else
		public event EventHandler CanExecuteChanged;
#endif
	}

	public class RelayCommand<T> : NotifyPropertyChanged, ICommand
	{
		private readonly Action<T> execute;
		private readonly Predicate<T> canExecute;

		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			this.execute = execute;
			this.canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return canExecute == null ? true : canExecute((T)parameter);
		}

		public void Execute(object parameter)
		{
			execute((T)parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			RaisePropertyChanged("CanExecute");
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, new EventArgs());
		}

#if METRO
		public event Windows.UI.Xaml.EventHandler CanExecuteChanged;
#else
		public event EventHandler CanExecuteChanged;
#endif
	} 
}
