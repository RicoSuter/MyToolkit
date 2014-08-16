//-----------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows.Input;

namespace MyToolkit.Command
{
    /// <summary>
    /// Provides an implementation of the <see cref="ICommand"/> interface. 
    /// </summary>
    public class RelayCommand : CommandBase
	{
        private readonly Action _execute;
		private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute)
			: this(execute, null) { }

        public RelayCommand(Action execute, Func<bool> canExecute)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
		}

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        protected override void Execute()
        {
            _execute();
        }

        /// <summary>
        /// Gets a value indicating whether the command can execute in its current state.
        /// </summary>
        public override bool CanExecute 
		{
            get { return _canExecute == null || _canExecute(); }
		}
	}

    /// <summary>
    /// Provides an implementation of the <see cref="ICommand"/> interface. 
    /// </summary>
    /// <typeparam name="T">The type of the command parameter. </typeparam>
	public class RelayCommand<T> : CommandBase<T>
    {
		private readonly Action<T> _execute;
		private readonly Predicate<T> _canExecute;

		public RelayCommand(Action<T> execute)
			: this(execute, null)
		{
		}

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Gets a value indicating whether the command can execute in its current state.
        /// </summary>
        [DebuggerStepThrough]
        public override bool CanExecute(T parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        protected override void Execute(T parameter)
        {
            _execute(parameter);
        }
	} 
}
