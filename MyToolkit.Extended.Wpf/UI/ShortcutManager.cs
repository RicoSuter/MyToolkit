//-----------------------------------------------------------------------
// <copyright file="ShortcutManager.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using MyToolkit.Command;

namespace MyToolkit.UI
{
    // TODO: Document ShortcutManager

    /// <summary>The manager for registering shortcuts.</summary>
    public static class ShortcutManager
    {
        /// <summary>Registers a given shortcut for a specific view and connect that shortcut with a given action.</summary>
        /// <param name="viewType">The type of the view.</param>
        /// <param name="gesture">The shortcut.</param>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">Delegate that determines wheter the action can be executed.</param>
        public static void RegisterShortcut(Type viewType, KeyGesture gesture, Action action, Func<bool> canExecute = null)
        {
            CommandManager.RegisterClassInputBinding(
                viewType,
                canExecute == null ? new InputBinding(new RelayCommand(action), gesture) : new InputBinding(new RelayCommand(action, canExecute), gesture));
        }

        /// <summary>Registers a given shortcut for a specific view and connect that shortcut with a given action.</summary>
        /// <param name="viewType">The type of the view.</param>
        /// <param name="gesture">The shortcut.</param>
        /// <param name="command">The command.</param>
        public static void RegisterShortcut(Type viewType, KeyGesture gesture, ICommand command)
        {
            Contract.Requires(command != null);

            CommandManager.RegisterClassInputBinding(
                viewType, new InputBinding(command, gesture));
        }

        /// <summary>Registers a given shortcut for specific views and connect that shortcut with a given action.</summary>
        /// <param name="viewTypes">The view types.</param>
        /// <param name="gesture">The shortcut.</param>
        /// <param name="action">The action.</param>
        /// <param name="canExecute">Delegate that determines wheter the action can be executed.</param>
        public static void RegisterShortcut(Type[] viewTypes, KeyGesture gesture, Action action, Func<bool> canExecute = null)
        {
            foreach (var type in viewTypes)
                RegisterShortcut(type, gesture, action, canExecute);
        }

        /// <summary>Registers a given shortcut for a specific views and connect that shortcut with a given action.</summary>
        /// <param name="viewTypes">The view types.</param>
        /// <param name="gesture">The shortcut.</param>
        /// <param name="command">The command.</param>
        public static void RegisterShortcut(Type[] viewTypes, KeyGesture gesture, ICommand command)
        {
            Contract.Requires(command != null);

            foreach (var type in viewTypes)
                RegisterShortcut(type, gesture, command);
        }
    }

}
