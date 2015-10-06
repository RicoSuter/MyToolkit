//-----------------------------------------------------------------------
// <copyright file="UndoRedoManager.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using MyToolkit.Collections;
using MyToolkit.Model;
using MyToolkit.Mvvm;

namespace MyToolkit.Data
{
    /// <summary>Manages undo/redo for a <see cref="GraphObservableObject"/> object. 
    /// Only classes which inherit from GraphObservableObject and properties which are 
    /// changed via the Set method or raise <see cref="GraphPropertyChangedEventArgs"/> are tracked for undo/redo. 
    /// Properties must have an accessible setter. Collection must inherit from INotifyCollectionChanged for
    /// undo/redo of collection changes. The class is not thread-safe; all methods must be called on the UI thread. </summary>
    public class UndoRedoManager : ObservableObject
    {
        private readonly List<List<UndoRedoCommand>> _commands = new List<List<UndoRedoCommand>>();
        private readonly IDispatcher _dispatcher;
        private readonly string[] _excludedRootProperties;
        private readonly GraphObservableObject _root;

        private int _currentIndex = -1;
        private bool _trackEntities = true;
        private List<UndoRedoCommand> _currentCommandSet;
        private int _restorePoint = -1;

        /// <summary>Initializes a new instance of the <see cref="UndoRedoManager"/> class.</summary>
        /// <param name="root">The root.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="excludedRootProperties">The excluded root properties.</param>
        public UndoRedoManager(GraphObservableObject root, IDispatcher dispatcher, string[] excludedRootProperties = null)
        {
            _root = root;
            _dispatcher = dispatcher;
            _excludedRootProperties = excludedRootProperties ?? new string[] { };

            root.GraphPropertyChanged += OnGraphPropertyChanged;
        }

        /// <summary>Initializes a new instance of the <see cref="UndoRedoManager"/> class.</summary>
        /// <param name="root">The root.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="excludedRootProperties">The excluded root properties.</param>
        public UndoRedoManager(INotifyCollectionChanged root, IDispatcher dispatcher, string[] excludedRootProperties = null)
            : this(new ObservableCollectionWrapper { Collection = root }, dispatcher, excludedRootProperties)
        {
        }

        /// <summary>Gets a value indicating whether the last action can be reverted. </summary>
        public bool CanUndo
        {
            get { return _currentIndex >= 0; }
        }

        /// <summary>Gets a value indicating whether the last reverted action can be repeated. </summary>
        public bool CanRedo
        {
            get { return _commands.Count > 0 && _currentIndex < _commands.Count - 1; }
        }

        /// <summary>Gets the the current index. </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
        }

        /// <summary>Removes all tracked changes and resets the restore point and current index. </summary>
        public void Reset()
        {
            _restorePoint = -1;
            _currentIndex = -1;

            if (_currentCommandSet != null)
                _currentCommandSet.Clear();

            RaiseAllUndoRedoPropertiesChanged();
        }

        /// <summary>Reverts the last action. </summary>
        /// <returns>A value indicating whether the undo could be performed. </returns>
        public bool Undo()
        {
            if (CanUndo)
            {
                _trackEntities = false;
                foreach (var command in _commands[_currentIndex])
                    command.Undo();

                _currentIndex--;
                _trackEntities = true;

                RaiseAllUndoRedoPropertiesChanged();
                return true;
            }
            return false;
        }

        /// <summary>Repeats the last reverted action. </summary>
        /// <returns>A value indicating whether the redo could be performed. </returns>
        public bool Redo()
        {
            if (CanRedo)
            {
                _trackEntities = false;
                _currentIndex++;

                foreach (var command in _commands[_currentIndex])
                    command.Redo();

                _trackEntities = true;

                RaiseAllUndoRedoPropertiesChanged();
                return true;
            }
            return false;
        }

        /// <summary>Sets the restore point to the current index. </summary>
        public void CreateRestorePoint()
        {
            _restorePoint = _currentIndex;
        }

        /// <summary>Reverts back to the last saved restore point (or to the initial state of the root object). </summary>
        /// <returns>True if reversion worked. </returns>
        public bool RevertToRestorePoint()
        {
            return Revert(_restorePoint);
        }

        /// <summary>Reverts back to the given index. </summary>
        /// <returns>True if reversion worked. </returns>
        public bool Revert(int index)
        {
            if (_currentIndex != -1 && index < _commands.Count - 1)
            {
                while (_currentIndex != index)
                    Undo();

                return true;
            }
            return false;
        }

        private void OnGraphPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (!_trackEntities)
                return;

            if (sender == _root && _excludedRootProperties.Contains(args.PropertyName))
                return;

            if (args is GraphPropertyChangedEventArgs || args is IExtendedNotifyCollectionChangedEventArgs)
            {
                var callDispatcher = false;
                if (_currentCommandSet == null)
                {
                    _currentCommandSet = new List<UndoRedoCommand>();
                    callDispatcher = true;
                }

                RemoveRedoCommands();
                _currentCommandSet.Add(UndoRedoCommand.CreateFromEventArgs(sender, args));

                if (callDispatcher)
                {
                    _dispatcher.InvokeAsync(() =>
                    {
                        _commands.Add(_currentCommandSet);
                        _currentCommandSet = null;
                        _currentIndex++;

                        RaiseAllUndoRedoPropertiesChanged();
                    });
                }
            }
        }

        private void RemoveRedoCommands()
        {
            if (_commands.Count > _currentIndex + 1)
                _commands.RemoveRange(_currentIndex + 1, _commands.Count - _currentIndex - 1);
        }

        private void RaiseAllUndoRedoPropertiesChanged()
        {
            RaisePropertyChanged(() => CanUndo);
            RaisePropertyChanged(() => CanRedo);
            RaisePropertyChanged(() => CurrentIndex);
        }
    }

    internal abstract class UndoRedoCommand
    {
        public UndoRedoCommand(object sender)
        {
            Sender = sender;
        }

        public object Sender { get; private set; }

        public abstract void Undo();

        public abstract void Redo();

        public static UndoRedoCommand CreateFromEventArgs(object sender, PropertyChangedEventArgs args)
        {
            if (args is IExtendedNotifyCollectionChangedEventArgs)
            {
                var collectionArgs = (IExtendedNotifyCollectionChangedEventArgs)args;
                return new CollectionCommand(sender, collectionArgs.OldCollection, ((ICollection)sender).OfType<object>().ToList());
            }
            else
            {
                var propertyArgs = (GraphPropertyChangedEventArgs)args;
                return new PropertyCommand(sender, propertyArgs.PropertyName, propertyArgs.OldValue, propertyArgs.NewValue);
            }
        }
    }

    internal class PropertyCommand : UndoRedoCommand
    {
        public PropertyCommand(object sender, string propertyName, object oldValue, object newValue)
            : base(sender)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public string PropertyName { get; private set; }

        public object OldValue { get; private set; }

        public object NewValue { get; private set; }

        public override void Undo()
        {
#if !LEGACY
            var property = Sender.GetType().GetRuntimeProperty(PropertyName);
            property.SetValue(Sender, OldValue);
#else
            var property = Sender.GetType().GetProperty(PropertyName);
            property.SetValue(Sender, OldValue, null);
#endif
        }

        public override void Redo()
        {
#if !LEGACY
            var property = Sender.GetType().GetRuntimeProperty(PropertyName);
            property.SetValue(Sender, NewValue);
#else
            var property = Sender.GetType().GetProperty(PropertyName);
            property.SetValue(Sender, NewValue, null);
#endif
        }
    }

    internal class CollectionCommand : UndoRedoCommand
    {
        public CollectionCommand(object sender, IEnumerable oldCollection, IEnumerable newCollection)
            : base(sender)
        {
            OldCollection = oldCollection;
            NewCollection = newCollection;
        }

        public IEnumerable OldCollection { get; private set; }

        public IEnumerable NewCollection { get; private set; }

        public override void Undo()
        {
            if (Sender is IList)
            {
                ((IList)Sender).Clear();
                foreach (var item in OldCollection)
                    ((IList)Sender).Add(item);
            }
            else
            {
#if LEGACY
                ((IDictionary)Sender).Clear();
                foreach (object item in OldCollection)
                    ((IDictionary)Sender).Add(
                        item.GetType().GetProperty("Key").GetValue(item, null), 
                        item.GetType().GetProperty("Value").GetValue(item, null));
#else
                ((IDictionary)Sender).Clear();
                foreach (dynamic item in OldCollection)
                    ((IDictionary)Sender).Add(item.Key, item.Value);
#endif
            }
        }

        public override void Redo()
        {
            if (Sender is IList)
            {
                ((IList)Sender).Clear();
                foreach (var item in NewCollection)
                    ((IList)Sender).Add(item);
            }
            else
            {
#if LEGACY
                ((IDictionary)Sender).Clear();
                foreach (object item in NewCollection)
                    ((IDictionary)Sender).Add(
                        item.GetType().GetProperty("Key").GetValue(item, null),
                        item.GetType().GetProperty("Value").GetValue(item, null));
#else
                ((IDictionary)Sender).Clear();
                foreach (dynamic item in NewCollection)
                    ((IDictionary)Sender).Add(item.Key, item.Value);
#endif
            }
        }
    }

    internal class ObservableCollectionWrapper : GraphObservableObject
    {
        private INotifyCollectionChanged _collection;

        public INotifyCollectionChanged Collection
        {
            get { return _collection; }
            set { Set(ref _collection, value); }
        }
    }
}
