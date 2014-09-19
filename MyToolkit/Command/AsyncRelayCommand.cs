//-----------------------------------------------------------------------
// <copyright file="AsyncRelayCommand.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyToolkit.Command
{
    /// <summary>Provides an async implementation of the <see cref="ICommand"/> interface. 
    /// The command is inactive when the command's task is running. </summary>
    public class AsyncRelayCommand : CommandBase
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isRunning = false;
        
        public AsyncRelayCommand(Func<Task> execute)
            : this(execute, null) { }

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>Gets a value indicating whether the command is currently running. </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                _isRunning = value;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>Defines the method to be called when the command is invoked. </summary>
        protected override async void Execute()
        {
            var task = _execute();
            if (task != null)
            {
                IsRunning = true;
                await task;
                IsRunning = false;
            }
        }

        /// <summary>Gets a value indicating whether the command can execute in its current state. </summary>
        public override bool CanExecute 
        {
            get { return !IsRunning && (_canExecute == null || _canExecute()); }
        }
    }

    /// <summary>Provides an implementation of the <see cref="ICommand"/> interface. </summary>
    /// <typeparam name="T">The type of the command parameter. </typeparam>
    public class AsyncRelayCommand<T> : CommandBase<T>
    {
        private readonly Func<T, Task> _execute;
        private readonly Predicate<T> _canExecute;
        private bool _isRunning = false;

        public AsyncRelayCommand(Func<T, Task> execute)
            : this(execute, null)
        {
        }

        public AsyncRelayCommand(Func<T, Task> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>Gets a value indicating whether the command is currently running. </summary>
        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                _isRunning = value;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>Gets a value indicating whether the command can execute in its current state. </summary>
        [DebuggerStepThrough]
        public override bool CanExecute(T parameter)
        {
            return !IsRunning && (_canExecute == null || _canExecute(parameter));
        }

        /// <summary>Defines the method to be called when the command is invoked. </summary>
        protected override async void Execute(T parameter)
        {
            var task = _execute(parameter);
            if (task != null)
            {
                IsRunning = true;
                await task;
                IsRunning = false;
            }
        }
    } 
}